// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Helpers;
using ScrcpyLauncherForCommandPalette.Pages;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Features.Diagnostic;

public sealed record AppInfo(string PackageName, string AppName)
{
    public string DisplayName => $"{AppName} ({PackageName})";
}

public sealed class ListApps : IScrcpyFeature, IScrcpyCustomPageFeature
{
    private static readonly ConcurrentDictionary<string, List<AppInfo>> _cache = new(StringComparer.OrdinalIgnoreCase);

    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.list-apps",
            Title = "List Installed Apps",
            Arguments = "--list-apps",
            Description = "List all Android applications installed on the device",
            Category = "Diagnostic",
            Icon = Icon.Terminal,
            RequiresConsole = true,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#start-android-app",
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        var serial = service.State?.DeviceSerial;
        var args = string.IsNullOrWhiteSpace(serial) ? "--list-apps" : $"-s {serial} --list-apps";
        return new ScrcpyCommandOutputPage("List Installed Apps", args, Icon.Terminal);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }

    public static bool TryGetCachedApps(string? serial, out List<AppInfo> apps)
    {
        string key = serial ?? string.Empty;
        return _cache.TryGetValue(key, out apps!);
    }

    public static List<AppInfo> GetCachedOrFetchApps(string? serial, bool forceRefresh = false)
    {
        string key = serial ?? string.Empty;
        if (!forceRefresh && _cache.TryGetValue(key, out var cached) && cached.Count > 0)
        {
            return cached;
        }

        var apps = GetInstalledApps(serial);
        if (apps.Count > 0)
        {
            _cache[key] = apps;
        }
        return apps;
    }

    public static List<AppInfo> GetInstalledApps(string? serial = null)
    {
        var apps = new List<AppInfo>();
        var seenPackages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1. Try running scrcpy --list-apps
        try
        {
            var scrcpyExe = SettingsManager.Instance?.ScrcpyPath?.Trim()
                ?? ScrcpyRunner.SettingsManager?.ScrcpyPath?.Trim();
            if (string.IsNullOrEmpty(scrcpyExe) || !File.Exists(scrcpyExe))
            {
                scrcpyExe = "scrcpy";
            }

            var args = string.IsNullOrEmpty(serial) ? "--list-apps" : $"-s {serial} --list-apps";
            var psi = new ProcessStartInfo
            {
                FileName = scrcpyExe,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
                CreateNoWindow = true,
            };

            using var proc = Process.Start(psi);
            if (proc != null)
            {
                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit(7000);

                ParseAppsFromOutput(stdout, apps, seenPackages);
                if (apps.Count == 0 && !string.IsNullOrWhiteSpace(stderr))
                {
                    ParseAppsFromOutput(stderr, apps, seenPackages);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"[ListApps] Error running scrcpy --list-apps: {ex.Message}");
        }

        // 2. If scrcpy --list-apps didn't yield packages, fall back to AdbHelper
        if (apps.Count == 0)
        {
            try
            {
                var pkgInfos = AdbHelper.GetInstalledPackages(serial);
                foreach (var pkg in pkgInfos)
                {
                    if (seenPackages.Add(pkg.Name))
                    {
                        apps.Add(new AppInfo(pkg.Name, pkg.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }

        return [.. apps.OrderBy(a => a.AppName, StringComparer.OrdinalIgnoreCase).ThenBy(a => a.PackageName, StringComparer.OrdinalIgnoreCase)];
    }

    private static void ParseAppsFromOutput(string output, List<AppInfo> apps, HashSet<string> seenPackages)
    {
        if (string.IsNullOrWhiteSpace(output)) return;

        var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        bool inListOfApps = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Recognize start of the apps list in scrcpy output
            if (line.Contains("List of apps", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("List of installed applications", StringComparison.OrdinalIgnoreCase))
            {
                inListOfApps = true;
                continue;
            }

            // Always ignore scrcpy banners (e.g. "scrcpy 4.1 <https://github.com/...>"), logging, and push status
            if (line.StartsWith("scrcpy", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("INFO:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("DEBUG:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("WARN:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("[server]", StringComparison.OrdinalIgnoreCase) ||
                line.StartsWith("* daemon", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Processing Android apps", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("file pushed", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("http://", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("https://", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("github.com", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // In scrcpy 3.x/4.x:
            // " * App vault                      com.mi.globalminusscreen"
            if (line.StartsWith('*'))
            {
                line = line.TrimStart('*', ' ', '\t');
            }
            else if (line.StartsWith('-'))
            {
                line = line.TrimStart('-', ' ', '\t');
            }
            else if (!inListOfApps)
            {
                // If not yet reached "List of apps:" header and line has no bullet '*', skip banner/device lines
                continue;
            }

            int lastSpace = line.LastIndexOfAny([' ', '\t']);
            int firstSpace = line.IndexOfAny([' ', '\t']);
            if (lastSpace <= 0) continue;

            string tokenEnd = line[(lastSpace + 1)..].Trim('"', '\'', '`', ' ');
            string restBeforeEnd = line[..lastSpace].Trim('"', '\'', '`', ' ');

            string pkg = string.Empty;
            string appName = string.Empty;

            // In scrcpy: " * <App Name>        <package.name>"
            // The package name is typically the last token.
            if (IsValidPackageName(tokenEnd))
            {
                pkg = tokenEnd;
                appName = restBeforeEnd;
            }
            else
            {
                string tokenStart = line[..firstSpace].Trim('"', '\'', '`', ' ');
                string restAfterStart = line[(firstSpace + 1)..].Trim('"', '\'', '`', ' ');
                if (IsValidPackageName(tokenStart))
                {
                    pkg = tokenStart;
                    appName = restAfterStart;
                }
                else
                {
                    continue;
                }
            }

            if (!string.IsNullOrWhiteSpace(pkg) && seenPackages.Add(pkg))
            {
                string cleanAppName = string.IsNullOrWhiteSpace(appName) ? pkg : appName;
                apps.Add(new AppInfo(pkg, cleanAppName));
            }
        }
    }

    private static bool IsValidPackageName(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        if (!token.Contains('.')) return false;
        if (token.StartsWith('.') || token.EndsWith('.')) return false;
        if (token.Contains('/') || token.Contains('\\') || token.Contains(':') ||
            token.Contains('<') || token.Contains('>') || token.Contains(' ') ||
            token.Contains('?') || token.Contains('*') || token.Contains('"') ||
            token.Contains('\'') || token.Contains('(') || token.Contains(')'))
        {
            return false;
        }

        var segments = token.Split('.');
        if (segments.Length < 2) return false;
        foreach (var seg in segments)
        {
            if (string.IsNullOrEmpty(seg)) return false;
            foreach (char c in seg)
            {
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
            }
        }

        return true;
    }
}