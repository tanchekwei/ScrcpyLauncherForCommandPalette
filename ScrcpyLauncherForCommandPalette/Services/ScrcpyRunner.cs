// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Enums;

namespace ScrcpyLauncherForCommandPalette.Services;

public static class ScrcpyRunner
{
    private static SettingsManager? _settingsManager;
    public static SettingsManager? SettingsManager { get; set; }

    public static void Initialize(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        SettingsManager = settingsManager;
    }

    public static CommandResult Run(string arguments, bool? inConsole = null, bool runAsAdmin = false)
    {
        var scrcpyExe = _settingsManager?.ScrcpyPath?.Trim();
        try
        {
            var defaultArgs = _settingsManager?.DefaultArguments?.Trim() ?? string.Empty;
            var cleanArgs = arguments?.Trim() ?? string.Empty;

            var combinedArgs = string.IsNullOrEmpty(defaultArgs)
                ? cleanArgs
                : (string.IsNullOrEmpty(cleanArgs) ? defaultArgs : $"{defaultArgs} {cleanArgs}");

            var finalInConsole = (inConsole is null && _settingsManager.RunScrcpyInConsole) || (inConsole is not null && inConsole == true);

            if (string.IsNullOrEmpty(scrcpyExe))
            {
                if (finalInConsole)
                {
                    scrcpyExe = "scrcpy";
                }
                else
                {
                    scrcpyExe = "scrcpy-noconsole.vbs";
                }
            }
            else
            {
                if (File.Exists(scrcpyExe))
                {
                    if (!finalInConsole)
                    {
                        scrcpyExe = scrcpyExe.Replace("scrcpy.exe", "scrcpy-noconsole.vbs");
                    }
                }
                else
                {
                    new ToastStatusMessage($"{scrcpyExe} was not found. Please ensure scrcpy path is correct.").Show();
                }
            }

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
            };

            if (runAsAdmin)
            {
                startInfo.Verb = "runas";
            }

            if (finalInConsole)
            {
                var terminalType = _settingsManager?.TerminalType ?? TerminalType.Cmd;
                switch (terminalType)
                {
                    case TerminalType.WindowsTerminal:
                        startInfo.FileName = "wt.exe";
                        startInfo.Arguments = string.IsNullOrEmpty(combinedArgs)
                            ? $"cmd.exe /k \"{scrcpyExe}\""
                            : $"cmd.exe /k \"\"{scrcpyExe}\" {combinedArgs}\"";
                        break;

                    case TerminalType.PowerShell:
                        startInfo.FileName = "powershell.exe";
                        startInfo.Arguments = string.IsNullOrEmpty(combinedArgs)
                            ? $"-NoExit -Command \"& '{scrcpyExe}'\""
                            : $"-NoExit -Command \"& '{scrcpyExe}' {combinedArgs}\"";
                        break;

                    case TerminalType.Cmd:
                    default:
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = string.IsNullOrEmpty(combinedArgs)
                            ? $"/k \"{scrcpyExe}\""
                            : $"/k \"\"{scrcpyExe}\" {combinedArgs}\"";
                        break;
                }
            }
            else
            {
                startInfo.FileName = scrcpyExe;
                startInfo.Arguments = combinedArgs;
            }

            Process.Start(startInfo);
            return CommandResult.Dismiss();
        }
        catch (Win32Exception win32Ex)
        {
            if (win32Ex.NativeErrorCode == 2) // File not found
            {
                new ToastStatusMessage($"{scrcpyExe} was not found. Please ensure scrcpy is installed and added to PATH, or set its path in settings.").Show();
            }
            else if (win32Ex.NativeErrorCode == 1223) // Operation cancelled by user
            {
                return CommandResult.KeepOpen();
            }
            else
            {
                ErrorLogger.LogError(win32Ex);
                new ToastStatusMessage($"Failed to launch scrcpy: {win32Ex.Message}").Show();
            }
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            new ToastStatusMessage($"Error launching scrcpy: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }

    public static string RunAndGetOutput(string arguments, int timeoutMs = 7000)
    {
        try
        {
            var scrcpyExe = _settingsManager?.ScrcpyPath?.Trim()
                ?? SettingsManager?.ScrcpyPath?.Trim()
                ?? SettingsManager.Instance?.ScrcpyPath?.Trim();

            if (string.IsNullOrEmpty(scrcpyExe) || !File.Exists(scrcpyExe))
            {
                scrcpyExe = "scrcpy";
            }

            var psi = new ProcessStartInfo
            {
                FileName = scrcpyExe,
                Arguments = arguments?.Trim() ?? string.Empty,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return string.Empty;
            }

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            if (process.WaitForExit(timeoutMs))
            {
                var stdout = stdoutTask.GetAwaiter().GetResult();
                var stderr = stderrTask.GetAwaiter().GetResult();

                if (!string.IsNullOrWhiteSpace(stdout) && !string.IsNullOrWhiteSpace(stderr))
                {
                    return $"{stdout.Trim()}{Environment.NewLine}{stderr.Trim()}";
                }

                return !string.IsNullOrWhiteSpace(stdout) ? stdout.Trim() : stderr.Trim();
            }
            else
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }

                return "Command timed out.";
            }
        }
        catch (Win32Exception win32Ex)
        {
            if (win32Ex.NativeErrorCode == 2)
            {
                return "scrcpy was not found. Please ensure scrcpy is installed and added to PATH, or set its path in settings.";
            }

            ErrorLogger.LogError(win32Ex);
            return $"Failed to run scrcpy: {win32Ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return $"Error: {ex.Message}";
        }
    }

    public static string GetConsoleOutput(string arguments, int timeoutMs = 7000) =>
        RunAndGetOutput(arguments, timeoutMs);

    public static void CopyToClipboard(string text)
    {
        try
        {
            var thread = new System.Threading.Thread(() =>
            {
                try
                {
                    OpenClipboard(IntPtr.Zero);
                    EmptyClipboard();
                    IntPtr hGlobal = Marshal.StringToHGlobalUni(text);
                    SetClipboardData(13, hGlobal); // CF_UNICODETEXT = 13
                    CloseClipboard();
                }
                catch
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "powershell",
                            Arguments = $"-command \"Set-Clipboard -Value '{text.Replace("'", "''")}'\"",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                        };
                        Process.Start(psi)?.WaitForExit(1500);
                    }
                    catch
                    {
                    }
                }
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join(1000);

            new ToastStatusMessage("Copied to clipboard").Show();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
}