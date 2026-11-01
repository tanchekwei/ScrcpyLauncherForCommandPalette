// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Services;

public sealed partial class AutoAdbService : IDisposable
{
    private const int MaxRetries = 3;
    private static readonly Lazy<AutoAdbService> _lazy = new(() => new AutoAdbService());
    public static AutoAdbService Instance => _lazy.Value;

    private readonly Dictionary<string, Process> _runningProcesses = new();
    private readonly Dictionary<string, CancellationTokenSource> _retryCts = new();
    private readonly Dictionary<string, int> _retryCounts = new();
    private readonly Dictionary<string, List<string>> _consoleLogs = new();
    private const int MaxLogLinesPerCommand = 500;
    private readonly object _lock = new();
    private bool _isDisposed;

    public event EventHandler? StatusChanged;

    public void AppendConsoleLog(string id, string line)
    {
        lock (_lock)
        {
            if (!_consoleLogs.TryGetValue(id, out var list))
            {
                list = new List<string>();
                _consoleLogs[id] = list;
            }

            list.Add(line);

            if (list.Count > MaxLogLinesPerCommand)
            {
                list.RemoveAt(0);
            }
        }
    }

    public string GetConsoleOutput(string id)
    {
        lock (_lock)
        {
            if (_consoleLogs.TryGetValue(id, out var list) && list.Count > 0)
            {
                return string.Join(Environment.NewLine, list);
            }
            return string.Empty;
        }
    }

    public int RunningCount
    {
        get
        {
            lock (_lock)
            {
                return _runningProcesses.Count(p =>
                {
                    try { return !p.Value.HasExited; }
                    catch { return false; }
                });
            }
        }
    }

    public bool IsRunning(string id)
    {
        lock (_lock)
        {
            if (_runningProcesses.TryGetValue(id, out var proc))
            {
                try
                {
                    if (!proc.HasExited)
                    {
                        return true;
                    }
                    _runningProcesses.Remove(id);
                }
                catch (Exception ex)
                {
                    Logger.Log($"[AutoAdbService.IsRunning] Error checking HasExited for {id}: {ex.Message}");
                    _runningProcesses.Remove(id);
                    return false;
                }
            }
            return false;
        }
    }

    public int? GetProcessId(string id)
    {
        lock (_lock)
        {
            if (_runningProcesses.TryGetValue(id, out var proc))
            {
                try
                {
                    if (!proc.HasExited)
                    {
                        return proc.Id;
                    }
                    _runningProcesses.Remove(id);
                }
                catch (Exception ex)
                {
                    Logger.Log($"[AutoAdbService.GetProcessId] Error checking process {id}: {ex.Message}");
                    _runningProcesses.Remove(id);
                }
            }
            return null;
        }
    }

    public bool Start(AutoAdbCommand cmd, bool isRetry = false)
    {
        Logger.Log($"[AutoAdbService.Start] Starting command '{cmd.Name}' (ID: {cmd.Id}), args: '{cmd.FormattedArguments}' (isRetry: {isRetry})");

        try
        {
            lock (_lock)
            {
                if (!isRetry)
                {
                    _retryCounts[cmd.Id] = 0;
                    if (_retryCts.TryGetValue(cmd.Id, out var existingCts))
                    {
                        try { existingCts.Cancel(); existingCts.Dispose(); } catch { }
                        _retryCts.Remove(cmd.Id);
                    }
                }
            }

            // First stop any previous instance for this ID
            StopInternal(cmd.Id, cancelRetry: !isRetry);

            var exePath = GetAutoAdbPath();
            Logger.Log($"[AutoAdbService.Start] Resolved autoadb path: '{exePath ?? "<null>"}'");

            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                Logger.Log($"[AutoAdbService.Start] ERROR: autoadb.exe not found!");
                new ToastStatusMessage("autoadb.exe not found in Assets folder.").Show();
                return false;
            }

            var scrcpyExe = SettingsManager.Instance?.ScrcpyPath;
            if (string.IsNullOrWhiteSpace(scrcpyExe) || !File.Exists(scrcpyExe))
            {
                scrcpyExe = "scrcpy";
            }
            Logger.Log($"[AutoAdbService.Start] Resolved scrcpy path: '{scrcpyExe}'");

            string procArgs = $"\"{scrcpyExe}\" {cmd.FormattedArguments}".Trim();
            Logger.Log($"[AutoAdbService.Start] Launching: '{exePath}' {procArgs}");

            bool showConsole = SettingsManager.Instance?.ShowAutoAdbConsole ?? false;

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = procArgs,
                UseShellExecute = false,
                CreateNoWindow = !showConsole,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process? proc;
            try
            {
                proc = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[AutoAdbService.Start] Process.Start threw exception: {ex}");
                new ToastStatusMessage($"Failed to start autoadb: {ex.Message}").Show();
                return false;
            }

            if (proc == null)
            {
                Logger.Log($"[AutoAdbService.Start] Process.Start returned null for '{cmd.Name}'");
                return false;
            }

            int procId = proc.Id;
            int retryTriggered = 0;
            CancellationTokenSource healthyCts = new();

            AppendConsoleLog(cmd.Id, $"[{DateTime.Now:HH:mm:ss.fff}] [system] Started autoadb PID: {procId}");
            Logger.Log($"[AutoAdbService.Start] Process started successfully. PID: {procId} for '{cmd.Name}'");

            if (showConsole)
            {
                try
                {
                    FreeConsole();
                    AttachConsole((uint)procId);
                }
                catch { }
            }

            proc.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    AppendConsoleLog(cmd.Id, $"[{DateTime.Now:HH:mm:ss.fff}] [stdout] {e.Data}");
                    Logger.Log($"[autoadb:{cmd.Name}:stdout] {e.Data}");
                    if (showConsole)
                    {
                        WriteToConsole(e.Data, isError: false);
                    }
                    if (e.Data.Contains("Server connection failed", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Interlocked.CompareExchange(ref retryTriggered, 1, 0) == 0)
                        {
                            try { healthyCts.Cancel(); } catch { }
                            ScheduleRetry(cmd, procId);
                        }
                    }
                }
            };

            proc.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    AppendConsoleLog(cmd.Id, $"[{DateTime.Now:HH:mm:ss.fff}] [stderr] {e.Data}");
                    Logger.Log($"[autoadb:{cmd.Name}:stderr] {e.Data}");
                    if (showConsole)
                    {
                        WriteToConsole(e.Data, isError: true);
                    }
                    if (e.Data.Contains("Server connection failed", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Interlocked.CompareExchange(ref retryTriggered, 1, 0) == 0)
                        {
                            try { healthyCts.Cancel(); } catch { }
                            ScheduleRetry(cmd, procId);
                        }
                    }
                }
            };

            proc.EnableRaisingEvents = true;
            proc.Exited += (s, e) =>
            {
                if (showConsole)
                {
                    try { FreeConsole(); } catch { }
                }

                try { healthyCts.Cancel(); } catch { }

                int exitCode = -1;
                try { exitCode = proc.ExitCode; } catch { }
                AppendConsoleLog(cmd.Id, $"[{DateTime.Now:HH:mm:ss.fff}] [system] Process exited with code: {exitCode}");
                Logger.Log($"[AutoAdbService] Process for '{cmd.Name}' (PID: {procId}) exited with code: {exitCode}");

                bool wasTracked;
                lock (_lock)
                {
                    wasTracked = _runningProcesses.Remove(cmd.Id);
                }

                // Only invoke StatusChanged if this was an unexpected/natural exit while actively tracked,
                // and NOT when explicitly killed by Stop/StopAll or during shutdown/disposal.
                if (wasTracked && !_isDisposed)
                {
                    try { StatusChanged?.Invoke(this, EventArgs.Empty); }
                    catch (Exception ex) { ErrorLogger.LogError(ex); }
                }
            };

            try
            {
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                Logger.Log($"[AutoAdbService.Start] Error reading process streams: {ex.Message}");
            }

            // Check if process exited immediately (e.g. invalid arguments or error)
            if (proc.WaitForExit(300))
            {
                try { healthyCts.Cancel(); } catch { }
                Logger.Log($"[AutoAdbService.Start] WARNING: autoadb process for '{cmd.Name}' exited immediately with code {proc.ExitCode}!");
                return false;
            }

            lock (_lock)
            {
                _runningProcesses[cmd.Id] = proc;
            }

            try
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }

            // Schedule reset of retry counter if process remains alive and running stably for 3 seconds
            var healthyToken = healthyCts.Token;
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(3000, healthyToken);
                    if (!proc.HasExited)
                    {
                        lock (_lock)
                        {
                            if (_runningProcesses.TryGetValue(cmd.Id, out var currentProc) && currentProc.Id == procId)
                            {
                                Logger.Log($"[AutoAdbService] Command '{cmd.Name}' has been running stably for 3s. Resetting retry count.");
                                _retryCounts[cmd.Id] = 0;
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
                catch { }
                finally
                {
                    healthyCts.Dispose();
                }
            });

            Logger.Log($"[AutoAdbService.Start] Command '{cmd.Name}' is actively running in background (PID: {proc.Id}).");
            return true;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[AutoAdbService.Start] Unexpected error: {ex}");
            new ToastStatusMessage($"Failed to start autoadb: {ex.Message}").Show();
            return false;
        }
    }

    public void Stop(string id)
    {
        StopInternal(id, cancelRetry: true);
    }

    private void StopInternal(string id, bool cancelRetry)
    {
        Logger.Log($"[AutoAdbService.Stop] Requested stop for ID: {id} (cancelRetry: {cancelRetry})");
        Process? proc = null;

        lock (_lock)
        {
            if (cancelRetry)
            {
                if (_retryCts.TryGetValue(id, out var cts))
                {
                    try { cts.Cancel(); cts.Dispose(); } catch { }
                    _retryCts.Remove(id);
                }
                _retryCounts.Remove(id);
            }

            if (_runningProcesses.TryGetValue(id, out proc))
            {
                _runningProcesses.Remove(id);
                Logger.Log($"[AutoAdbService.Stop] Found running process for ID {id} in dictionary, removing tracking");
            }
            else
            {
                Logger.Log($"[AutoAdbService.Stop] No tracked process found for ID: {id}");
            }
        }

        if (proc != null)
        {
            try
            {
                FreeConsole();
            }
            catch { }

            try
            {
                bool hasExited = false;
                try { hasExited = proc.HasExited; }
                catch (Exception ex)
                {
                    Logger.Log($"[AutoAdbService.Stop] HasExited threw: {ex.Message}");
                    hasExited = true;
                }

                Logger.Log($"[AutoAdbService.Stop] Process PID: {proc.Id}, HasExited: {hasExited}");

                if (!hasExited)
                {
                    try
                    {
                        Logger.Log($"[AutoAdbService.Stop] Killing process tree for PID: {proc.Id}...");
                        proc.Kill(entireProcessTree: true);
                        Logger.Log($"[AutoAdbService.Stop] proc.Kill(entireProcessTree) completed for PID: {proc.Id}");
                    }
                    catch (Exception killEx)
                    {
                        ErrorLogger.LogError(killEx);
                        Logger.Log($"[AutoAdbService.Stop] proc.Kill(entireProcessTree) failed: {killEx.Message}. Trying standard proc.Kill()...");
                        try
                        {
                            proc.Kill();
                            Logger.Log($"[AutoAdbService.Stop] proc.Kill() completed for PID: {proc.Id}");
                        }
                        catch (Exception fallbackEx)
                        {
                            ErrorLogger.LogError(fallbackEx);
                            Logger.Log($"[AutoAdbService.Stop] Standard proc.Kill() failed: {fallbackEx.Message}");
                        }
                    }

                    try
                    {
                        proc.WaitForExit(1000);
                        Logger.Log($"[AutoAdbService.Stop] proc.WaitForExit finished for PID: {proc.Id}");
                    }
                    catch (Exception waitEx)
                    {
                        Logger.Log($"[AutoAdbService.Stop] proc.WaitForExit threw: {waitEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[AutoAdbService.Stop] Exception during process stop: {ex}");
            }
            finally
            {
                try
                {
                    proc.Dispose();
                    Logger.Log($"[AutoAdbService.Stop] Disposed process object for ID: {id}");
                }
                catch (Exception ex)
                {
                    Logger.Log($"[AutoAdbService.Stop] Process dispose threw: {ex.Message}");
                }
            }
        }

        if (!_isDisposed)
        {
            try
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }

        Logger.Log($"[AutoAdbService.Stop] Stop completed for ID: {id}");
    }

    public void StopAll()
    {
        Logger.Log("[AutoAdbService.StopAll] Stopping all running autoadb processes...");
        List<KeyValuePair<string, Process>> procsToStop;
        bool hadAnyProcessesOrRetries = false;

        lock (_lock)
        {
            if (_retryCts.Count > 0)
            {
                hadAnyProcessesOrRetries = true;
                foreach (var cts in _retryCts.Values)
                {
                    try { cts.Cancel(); cts.Dispose(); } catch { }
                }
                _retryCts.Clear();
                _retryCounts.Clear();
            }

            if (_runningProcesses.Count > 0)
            {
                hadAnyProcessesOrRetries = true;
            }

            procsToStop = [.. _runningProcesses];
            _runningProcesses.Clear();
        }

        Logger.Log($"[AutoAdbService.StopAll] Found {procsToStop.Count} process(es) to stop");

        foreach (var kvp in procsToStop)
        {
            var proc = kvp.Value;
            try
            {
                bool hasExited = false;
                try { hasExited = proc.HasExited; } catch { hasExited = true; }

                if (!hasExited)
                {
                    Logger.Log($"[AutoAdbService.StopAll] Killing process PID: {proc.Id} for command ID: {kvp.Key}");
                    try { proc.Kill(entireProcessTree: true); }
                    catch { try { proc.Kill(); } catch { } }
                    try { proc.WaitForExit(500); } catch { }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
            finally
            {
                try { proc.Dispose(); } catch { }
            }
        }

        // Only fire StatusChanged if something actually stopped/changed and we are not shutting down
        if (hadAnyProcessesOrRetries && !_isDisposed)
        {
            try
            {
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }

        Logger.Log("[AutoAdbService.StopAll] All autoadb processes stopped.");
    }

    public void StartAllEnabled(IEnumerable<AutoAdbCommand> commands)
    {
        var enabledList = commands.Where(c => c.IsEnabled).ToList();
        Logger.Log($"[AutoAdbService.StartAllEnabled] Starting {enabledList.Count} enabled commands...");
        foreach (var cmd in enabledList)
        {
            try
            {
                Start(cmd);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[AutoAdbService.StartAllEnabled] Error starting command '{cmd.Name}': {ex.Message}");
            }
        }
    }

    private void ScheduleRetry(AutoAdbCommand cmd, int procId)
    {
        Logger.Log($"[AutoAdbService.ScheduleRetry] 'Server connection failed' detected for '{cmd.Name}' (PID: {procId}). Preparing retry...");

        CancellationTokenSource cts;
        int currentRetries = 0;

        lock (_lock)
        {
            _retryCounts.TryGetValue(cmd.Id, out currentRetries);
            if (currentRetries >= MaxRetries)
            {
                Logger.Log($"[AutoAdbService.ScheduleRetry] Maximum retry attempts ({MaxRetries}) reached for '{cmd.Name}'. Aborting retry.");
                new ToastStatusMessage($"Connection failed for '{cmd.Name}' after {MaxRetries} retries.").Show();
                return;
            }

            _retryCounts[cmd.Id] = currentRetries + 1;

            if (_retryCts.TryGetValue(cmd.Id, out var existingCts))
            {
                try { existingCts.Cancel(); existingCts.Dispose(); } catch { }
            }

            cts = new CancellationTokenSource();
            _retryCts[cmd.Id] = cts;
        }

        int attemptNumber = currentRetries + 1;
        var token = cts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                AppendConsoleLog(cmd.Id, $"[{DateTime.Now:HH:mm:ss.fff}] [system] Connection failed. Retrying (Attempt {attemptNumber}/{MaxRetries})...");
                Logger.Log($"[AutoAdbService.ScheduleRetry] Retrying '{cmd.Name}' (Attempt {attemptNumber}/{MaxRetries})...");

                // 1. Kill current scrcpy
                KillCurrentScrcpy(procId, cmd);

                // 2. Stop current autoadb process without cancelling this retry CTS
                StopInternal(cmd.Id, cancelRetry: false);

                // 3. Wait ~300ms
                Logger.Log($"[AutoAdbService.ScheduleRetry] Waiting 300ms before restarting '{cmd.Name}'...");
                await Task.Delay(300, token);

                if (token.IsCancellationRequested)
                {
                    Logger.Log($"[AutoAdbService.ScheduleRetry] Retry canceled for '{cmd.Name}'.");
                    return;
                }

                // 4. Start autoadb again
                Logger.Log($"[AutoAdbService.ScheduleRetry] Restarting autoadb for '{cmd.Name}' now...");
                bool started = Start(cmd, isRetry: true);
                if (!started)
                {
                    Logger.Log($"[AutoAdbService.ScheduleRetry] Restart autoadb for '{cmd.Name}' returned false.");
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Log($"[AutoAdbService.ScheduleRetry] Retry canceled for '{cmd.Name}'.");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[AutoAdbService.ScheduleRetry] Error during retry for '{cmd.Name}': {ex.Message}");
            }
        }, token);
    }

    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool WriteConsole(
        IntPtr hConsoleOutput,
        string lpBuffer,
        uint nNumberOfCharsToWrite,
        out uint lpNumberOfCharsWritten,
        IntPtr lpReserved);

    private static void WriteToConsole(string text, bool isError = false)
    {
        try
        {
            var handle = GetStdHandle(isError ? STD_ERROR_HANDLE : STD_OUTPUT_HANDLE);
            if (handle != IntPtr.Zero && handle != INVALID_HANDLE_VALUE)
            {
                string line = text + Environment.NewLine;
                WriteConsole(handle, line, (uint)line.Length, out _, IntPtr.Zero);
            }
        }
        catch { }
    }

    private const uint TH32CS_SNAPPROCESS = 0x00000002;
    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static void KillCurrentScrcpy(int autoadbPid, AutoAdbCommand cmd)
    {
        Logger.Log($"[AutoAdbService.KillCurrentScrcpy] Searching for child scrcpy processes of autoadb PID {autoadbPid} ('{cmd.Name}')...");

        try
        {
            var snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (snapshot == INVALID_HANDLE_VALUE)
            {
                Logger.Log("[AutoAdbService.KillCurrentScrcpy] CreateToolhelp32Snapshot failed");
                return;
            }

            var processList = new List<(uint Pid, uint ParentPid, string ExeName)>();
            try
            {
                var entry = new PROCESSENTRY32 { dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32>() };
                if (Process32First(snapshot, ref entry))
                {
                    do
                    {
                        processList.Add((entry.th32ProcessID, entry.th32ParentProcessID, entry.szExeFile));
                    }
                    while (Process32Next(snapshot, ref entry));
                }
            }
            finally
            {
                CloseHandle(snapshot);
            }

            // Find all descendant process IDs belonging to autoadbPid
            var descendantPids = new HashSet<uint>();
            var queue = new Queue<uint>();
            queue.Enqueue((uint)autoadbPid);

            while (queue.Count > 0)
            {
                var currentParent = queue.Dequeue();
                foreach (var p in processList)
                {
                    if (p.ParentPid == currentParent && descendantPids.Add(p.Pid))
                    {
                        queue.Enqueue(p.Pid);
                    }
                }
            }

            // Filter for descendant processes whose executable is scrcpy
            var scrcpyDescendants = processList
                .Where(p => descendantPids.Contains(p.Pid) && p.ExeName.StartsWith("scrcpy", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (scrcpyDescendants.Count == 0)
            {
                Logger.Log($"[AutoAdbService.KillCurrentScrcpy] No child scrcpy process found for autoadb PID {autoadbPid}. Other scrcpy processes left untouched.");
                return;
            }

            Logger.Log($"[AutoAdbService.KillCurrentScrcpy] Found {scrcpyDescendants.Count} child scrcpy process(es) for autoadb PID {autoadbPid}");

            foreach (var target in scrcpyDescendants)
            {
                try
                {
                    var proc = Process.GetProcessById((int)target.Pid);
                    Logger.Log($"[AutoAdbService.KillCurrentScrcpy] Killing child scrcpy process PID: {target.Pid} ({target.ExeName})...");
                    proc.Kill(entireProcessTree: true);
                    proc.WaitForExit(500);
                    proc.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Log($"[AutoAdbService.KillCurrentScrcpy] Error killing child scrcpy PID {target.Pid}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[AutoAdbService.KillCurrentScrcpy] Exception while searching/killing child scrcpy: {ex.Message}");
        }
    }

    #if ARM64
        private const string AutoAdbFileName = "autoadb-arm64.exe";
    #else
        private const string AutoAdbFileName = "autoadb-x64.exe";
    #endif

    public static string? GetAutoAdbPath()
    {
        // 1. AppContext.BaseDirectory
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", AutoAdbFileName);
        if (File.Exists(path)) return path;

        // 2. AppDomain.CurrentDomain.BaseDirectory
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", AutoAdbFileName);
        if (File.Exists(path)) return path;

        // 3. Windows.ApplicationModel.Package.Current.InstalledLocation
        try
        {
            path = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "Assets",
                AutoAdbFileName);

            if (File.Exists(path)) return path;
        }
        catch { }

        // 4. Relative Assets path
        if (File.Exists(Path.Combine("Assets", AutoAdbFileName)))
            return Path.GetFullPath(Path.Combine("Assets", AutoAdbFileName));

        return null;
    }

    public void Dispose()
    {
        _isDisposed = true;
        StatusChanged = null; // Detach all listeners immediately on exit
        StopAll();
    }
}