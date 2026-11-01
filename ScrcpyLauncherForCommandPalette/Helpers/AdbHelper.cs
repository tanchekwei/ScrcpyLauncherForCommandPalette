// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Helpers;

public record DeviceInfo(string Serial, string Model, string State, bool IsTcpIp);
public record PackageInfo(string Name, bool IsDebuggable, bool IsRunning, bool IsForeground);

public static class AdbHelper
{
    // Runs adb, reading both stdout and stderr before WaitForExit to prevent deadlocks.
    public static void RunAdb(string arguments, out string stdoutOutput, out string stderrOutput)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "adb",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
            };

            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            bool hasError = process.ExitCode != 0
                || stderr.Contains("error:", StringComparison.OrdinalIgnoreCase);

            stdoutOutput = stdout;
            stderrOutput = hasError
                ? (string.IsNullOrWhiteSpace(stderr) ? $"adb exited with code {process.ExitCode}" : stderr.Trim())
                : string.Empty;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            stdoutOutput = string.Empty;
            stderrOutput = ex.Message;
        }
    }

    // Returns connected Android devices via "adb devices -l"
    public static DeviceInfo[] GetConnectedDevices()
    {
        try
        {
            RunAdb("devices -l", out string output, out string error);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrWhiteSpace(output))
            {
                return [];
            }

            var devices = new List<DeviceInfo>();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("List of devices", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("* daemon", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                // Line format: <serial> <state> [product:...] [model:...] [device:...] [transport_id:...]
                var parts = trimmed.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                var serial = parts[0];
                var state = parts[1];
                var model = serial; // fallback

                foreach (var part in parts.Skip(2))
                {
                    if (part.StartsWith("model:", StringComparison.OrdinalIgnoreCase))
                    {
                        model = part["model:".Length..].Replace('_', ' ');
                        break;
                    }
                }

                // Check if device is TCP/IP (contains IP:port pattern)
                bool isTcpIp = serial.Contains(':') && !serial.StartsWith("emulator", StringComparison.OrdinalIgnoreCase);

                devices.Add(new DeviceInfo(serial, model, state, isTcpIp));
            }

            return [.. devices];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return [];
        }
    }

    // Returns all installed 3rd-party packages with status flags via adb commands.
    public static List<PackageInfo> GetInstalledPackages(string? serial = null)
    {
        var result = new List<PackageInfo>();

        try
        {
            string prefix = string.IsNullOrEmpty(serial) ? "" : $"-s {serial} ";

            // 1. Get 3rd-party installed packages: "pm list packages -3"
            RunAdb($"{prefix}shell pm list packages -3", out string pmOutput, out string pmError);
            if (!string.IsNullOrEmpty(pmError) || string.IsNullOrWhiteSpace(pmOutput))
            {
                return result;
            }

            var packageNames = new List<string>();
            foreach (var raw in pmOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var line = raw.Trim();
                if (line.StartsWith("package:", StringComparison.OrdinalIgnoreCase))
                {
                    var name = line["package:".Length..].Trim();
                    if (!string.IsNullOrEmpty(name))
                        packageNames.Add(name);
                }
            }

            if (packageNames.Count == 0) return result;

            // 2. Query running packages via "ps -A"
            var runningSet = GetRunningPackages(serial);

            // 3. Query foreground package via dumpsys
            var foregroundPkg = GetForegroundPackage(serial);

            // 4. Batch query debuggable status from "dumpsys package packages"
            RunAdb($"{prefix}shell dumpsys package packages", out string dumpsysOutput, out _);
            var debuggableSet = ParseDebuggablePackages(dumpsysOutput);

            foreach (var name in packageNames)
            {
                bool isRunning = runningSet.Contains(name);
                bool isForeground = string.Equals(name, foregroundPkg, StringComparison.Ordinal);
                bool isDebuggable = debuggableSet.Contains(name);

                result.Add(new PackageInfo(name, isDebuggable, isRunning, isForeground));
            }

            // Foreground first, then running, then alphabetically
            return [.. result
                .OrderByDescending(p => p.IsForeground)
                .ThenByDescending(p => p.IsRunning)
                .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase)];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return result;
        }
    }

    // Returns the package name and main activity: e.g. "com.example/.MainActivity"
    public static string? GetMainActivity(string packageName, string? serial = null)
    {
        string prefix = string.IsNullOrEmpty(serial) ? "" : $"-s {serial} ";
        RunAdb($"{prefix}shell cmd package resolve-activity --brief {packageName}", out string output, out string error);
        if (!string.IsNullOrEmpty(error) || string.IsNullOrWhiteSpace(output))
            return null;

        foreach (var raw in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var line = raw.Trim();
            if (line.StartsWith(packageName, StringComparison.Ordinal))
                return line;
        }

        return null;
    }

    // Returns the set of currently running third-party app package names via "ps -A".
    private static HashSet<string> GetRunningPackages(string? serial = null)
    {
        string prefix = string.IsNullOrEmpty(serial) ? "" : $"-s {serial} ";
        RunAdb($"{prefix}shell ps -A", out string output, out string error);
        if (!string.IsNullOrEmpty(error))
            return [];

        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var raw in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var line = raw.Trim();
            // App processes run as u0_a* users
            if (!line.StartsWith("u0_a", StringComparison.Ordinal))
                continue;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 9)
                result.Add(parts[^1]);
        }

        return result;
    }

    // Returns the package name of the currently foregrounded app via "dumpsys window windows".
    private static string? GetForegroundPackage(string? serial = null)
    {
        string prefix = string.IsNullOrEmpty(serial) ? "" : $"-s {serial} ";
        RunAdb($"{prefix}shell dumpsys window windows", out string output, out string error);
        if (!string.IsNullOrEmpty(error))
            return null;

        foreach (var raw in output.Split('\n'))
        {
            var line = raw.Trim();
            if (!line.StartsWith("mCurrentFocus=", StringComparison.Ordinal))
                continue;

            var start = line.LastIndexOf(' ') + 1;
            var slash = line.IndexOf('/', start);
            var end = slash >= 0 ? slash : line.IndexOf('}', start);
            if (start > 0 && end > start)
                return line[start..end];
        }

        return null;
    }

    // Parses "dumpsys package packages" output and returns the set of debuggable package names.
    private static HashSet<string> ParseDebuggablePackages(string dumpsysOutput)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        string? current = null;

        foreach (var raw in dumpsysOutput.Split('\n'))
        {
            var line = raw.Trim();

            if (line.StartsWith("Package [", StringComparison.Ordinal))
            {
                var start = line.IndexOf('[') + 1;
                var end = line.IndexOf(']', start);
                if (start > 0 && end > start)
                    current = line[start..end];
            }
            else if (current != null
                && (line.StartsWith("flags=", StringComparison.Ordinal)
                    || line.StartsWith("pkgFlags=", StringComparison.Ordinal))
                && line.Contains("DEBUGGABLE", StringComparison.Ordinal))
            {
                result.Add(current);
            }
        }

        return result;
    }
}