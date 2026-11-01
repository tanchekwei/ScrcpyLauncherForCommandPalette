// Copyright (c) 2026 tanchekwei 
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Enums;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette;

public class SettingsManager : JsonSettingsManager
{
    private static readonly string _namespace = "scrcpy";

    private static string Namespaced(string propertyName) => $"{_namespace}.{propertyName}";

    public static SettingsManager? Instance { get; private set; }

    private static readonly List<ChoiceSetSetting.Choice> _terminalTypeChoices =
    [
        new ChoiceSetSetting.Choice("Windows Terminal", nameof(TerminalType.WindowsTerminal)),
        new ChoiceSetSetting.Choice("Command Prompt", nameof(TerminalType.Cmd)),
        new ChoiceSetSetting.Choice("PowerShell", nameof(TerminalType.PowerShell)),
    ];

    private readonly TextSetting _scrcpyPath = new(
        Namespaced(nameof(ScrcpyPath)),
        "Custom scrcpy Executable Path",
        "Optional path to scrcpy.exe. Leave empty to use system PATH.",
        "");

    private readonly TextSetting _defaultArguments = new(
        Namespaced(nameof(DefaultArguments)),
        "Default CLI Arguments",
        "Optional default CLI arguments passed to all scrcpy commands (e.g., --stay-awake).",
        "");

    private readonly ChoiceSetSetting _terminalType = new(
        Namespaced(nameof(TerminalType)),
        "Terminal Type",
        "The terminal to use when running scrcpy commands in a console window.",
        _terminalTypeChoices);

    private readonly ToggleSetting _runScrcpyInConsole = new(
        Namespaced(nameof(RunScrcpyInConsole)),
        "Run scrcpy in Console by default",
        "If uncheck, runs scrcpy without showing a console window using (scrcpy-noconsole.vbs) by default",
        false);

    private readonly ToggleSetting _showAutoAdbConsole = new(
        Namespaced(nameof(ShowAutoAdbConsole)),
        "Show AutoAdb Console",
        "Shows the console window when AutoAdb is running for debugging purposes?",
        false);

    private readonly ToggleSetting _enableLogging = new(
        Namespaced(nameof(EnableLogging)),
        "Enable Logging",
        "Enables diagnostic logging for troubleshooting.",
        false);

    private readonly List<ScrcpyFavoriteCommand> _favorites = [];
    private readonly object _favLock = new();

    private readonly List<AutoAdbCommand> _autoAdbCommands = [];
    private readonly object _autoAdbLock = new();

    public event EventHandler? FavoritesChanged;
    public event EventHandler? AutoAdbCommandsChanged;

    public string ScrcpyPath => _scrcpyPath.Value ?? string.Empty;
    public string DefaultArguments => _defaultArguments.Value ?? string.Empty;
    public bool EnableLogging => _enableLogging.Value;
    public bool ShowAutoAdbConsole => _showAutoAdbConsole.Value;
    public bool RunScrcpyInConsole => _runScrcpyInConsole.Value;

    public IReadOnlyList<ScrcpyFavoriteCommand> Favorites
    {
        get
        {
            lock (_favLock)
            {
                return [.. _favorites];
            }
        }
    }

    public IReadOnlyList<AutoAdbCommand> AutoAdbCommands
    {
        get
        {
            lock (_autoAdbLock)
            {
                return [.. _autoAdbCommands];
            }
        }
    }

    public TerminalType TerminalType
    {
        get
        {
            if (Enum.TryParse<TerminalType>(_terminalType.Value, out var result))
            {
                return result;
            }
            return TerminalType.Cmd;
        }
    }

    public static string GetJsonPath(string fileName)
    {
        try
        {
            var directory = Constant.SettingsFolderPath;
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, fileName);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            throw;
        }
    }

    internal static string SettingsJsonPath() => GetJsonPath("settings.json");
    internal static string FavoritesJsonPath() => GetJsonPath("favorites.json");
    internal static string AutoAdbJsonPath() => GetJsonPath("autoadb.json");

    private static List<ScrcpyFavoriteCommand> GetDefaultFavorites() =>
    [
        new ScrcpyFavoriteCommand
        {
            Id = "default",
            Name = "audio only",
            Arguments = "--no-window",
            Description = "--no-window",
            TargetDeviceSerial = null,
            TargetDeviceModel = null,
            CreatedAt = DateTime.UtcNow,
        },
    ];

    public SettingsManager()
    {
        try
        {
            Instance = this;
            FilePath = SettingsJsonPath();

            Settings.Add(_scrcpyPath);
            Settings.Add(_defaultArguments);
            Settings.Add(_terminalType);
            Settings.Add(_runScrcpyInConsole);
            Settings.Add(_showAutoAdbConsole);
            Settings.Add(_enableLogging);

            LoadSettings();
            LoadFavorites();
            LoadAutoAdbCommands();
            AutoAdbService.Instance.StartAllEnabled(_autoAdbCommands);

            Settings.SettingsChanged += (s, a) =>
            {
                SaveSettings();
            };
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }

    public void LoadFavorites()
    {
        lock (_favLock)
        {
            _favorites.Clear();
            try
            {
                string favPath = FavoritesJsonPath();

                if (!File.Exists(favPath))
                {
                    _favorites.AddRange(GetDefaultFavorites());
                    SaveFavorites();
                    return;
                }

                var json = File.ReadAllText(favPath);
                var favArray = JsonNode.Parse(json) as JsonArray;

                if (favArray != null)
                {
                    foreach (var item in favArray)
                    {
                        if (item is JsonObject obj)
                        {
                            var fav = new ScrcpyFavoriteCommand
                            {
                                Id = obj["Id"]?.ToString() ?? Guid.NewGuid().ToString("N"),
                                Name = obj["Name"]?.ToString() ?? string.Empty,
                                Arguments = obj["Arguments"]?.ToString() ?? string.Empty,
                                Description = obj["Description"]?.ToString() ?? string.Empty,
                                TargetDeviceSerial = obj["TargetDeviceSerial"]?.ToString(),
                                TargetDeviceModel = obj["TargetDeviceModel"]?.ToString(),
                                CreatedAt = DateTime.TryParse(obj["CreatedAt"]?.ToString(), out var dt) ? dt : DateTime.UtcNow,
                            };
                            _favorites.Add(fav);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }
    }

    public void SaveFavorites()
    {
        lock (_favLock)
        {
            try
            {
                var nodes = _favorites.Select(fav => (JsonNode)new JsonObject
                {
                    ["Id"] = fav.Id,
                    ["Name"] = fav.Name,
                    ["Arguments"] = fav.Arguments,
                    ["Description"] = fav.Description,
                    ["TargetDeviceSerial"] = fav.TargetDeviceSerial,
                    ["TargetDeviceModel"] = fav.TargetDeviceModel,
                    ["CreatedAt"] = fav.CreatedAt.ToString("o"),
                }).ToArray();

                var arr = new JsonArray(nodes);

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(FavoritesJsonPath(), arr.ToJsonString(options));

                FavoriteCommandsService.Instance.RebuildCache([.. _favorites], this);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }
    }

    public void AddFavorite(ScrcpyFavoriteCommand fav)
    {
        lock (_favLock)
        {
            _favorites.RemoveAll(f => f.Id == fav.Id || string.Equals(f.Name, fav.Name, StringComparison.OrdinalIgnoreCase));
            _favorites.Insert(0, fav);
            SaveFavorites();
        }
        FavoritesChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveFavorite(string id)
    {
        lock (_favLock)
        {
            _favorites.RemoveAll(f => f.Id == id);
            SaveFavorites();
        }
        FavoritesChanged?.Invoke(this, EventArgs.Empty);
    }

    public void LoadAutoAdbCommands()
    {
        lock (_autoAdbLock)
        {
            _autoAdbCommands.Clear();
            try
            {
                string autoAdbPath = AutoAdbJsonPath();
                Logger.Log($"[SettingsManager.LoadAutoAdbCommands] Loading from {autoAdbPath} or {FilePath}");
                JsonArray? autoArray = null;

                if (File.Exists(autoAdbPath))
                {
                    var json = File.ReadAllText(autoAdbPath);
                    autoArray = JsonNode.Parse(json) as JsonArray;
                }

                if (autoArray != null)
                {
                    foreach (var item in autoArray)
                    {
                        if (item is JsonObject obj)
                        {
                            bool isEnabled = true;
                            if (obj.TryGetPropertyValue("IsEnabled", out var enabledNode) && enabledNode != null)
                            {
                                if (bool.TryParse(enabledNode.ToString(), out var b))
                                {
                                    isEnabled = b;
                                }
                            }

                            var cmd = new AutoAdbCommand
                            {
                                Id = obj["Id"]?.ToString() ?? Guid.NewGuid().ToString("N"),
                                Name = obj["Name"]?.ToString() ?? string.Empty,
                                ScrcpyArguments = obj["ScrcpyArguments"]?.ToString() ?? string.Empty,
                                IsEnabled = isEnabled,
                                CreatedAt = DateTime.TryParse(obj["CreatedAt"]?.ToString(), out var dt) ? dt : DateTime.UtcNow,
                            };
                            _autoAdbCommands.Add(cmd);
                        }
                    }
                }
                Logger.Log($"[SettingsManager.LoadAutoAdbCommands] Loaded {_autoAdbCommands.Count} command(s)");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[SettingsManager.LoadAutoAdbCommands] Error loading: {ex.Message}");
            }
        }
    }

    public void SaveAutoAdbCommands()
    {
        lock (_autoAdbLock)
        {
            try
            {
                string autoAdbPath = AutoAdbJsonPath();
                Logger.Log($"[SettingsManager.SaveAutoAdbCommands] Saving {_autoAdbCommands.Count} command(s) to {autoAdbPath}");

                var nodes = _autoAdbCommands.Select(cmd => (JsonNode)new JsonObject
                {
                    ["Id"] = cmd.Id,
                    ["Name"] = cmd.Name,
                    ["ScrcpyArguments"] = cmd.ScrcpyArguments,
                    ["IsEnabled"] = cmd.IsEnabled,
                    ["CreatedAt"] = cmd.CreatedAt.ToString("o"),
                }).ToArray();

                var arr = new JsonArray(nodes);

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(autoAdbPath, arr.ToJsonString(options));
                Logger.Log($"[SettingsManager.SaveAutoAdbCommands] Successfully saved {_autoAdbCommands.Count} command(s)");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[SettingsManager.SaveAutoAdbCommands] Error saving: {ex.Message}");
            }
        }
    }

    public void AddAutoAdbCommand(AutoAdbCommand cmd)
    {
        Logger.Log($"[SettingsManager.AddAutoAdbCommand] Adding command '{cmd.Name}' (ID: {cmd.Id})");
        lock (_autoAdbLock)
        {
            _autoAdbCommands.RemoveAll(c => c.Id == cmd.Id || string.Equals(c.Name, cmd.Name, StringComparison.OrdinalIgnoreCase));
            _autoAdbCommands.Insert(0, cmd);
            SaveAutoAdbCommands();
        }

        if (cmd.IsEnabled)
        {
            try
            {
                AutoAdbService.Instance.Start(cmd);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[SettingsManager.AddAutoAdbCommand] Error starting process: {ex.Message}");
            }
        }

        try
        {
            AutoAdbCommandsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }

    public void RemoveAutoAdbCommand(string id)
    {
        Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Entering RemoveAutoAdbCommand for ID: {id}");

        // Stop background process safely with try-catch so it never blocks record deletion
        try
        {
            AutoAdbService.Instance.Stop(id);
            Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] AutoAdbService.Stop completed for ID: {id}");
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Exception caught during AutoAdbService.Stop: {ex}");
        }

        // Always remove the record and persist to settings file
        int removedCount = 0;
        lock (_autoAdbLock)
        {
            removedCount = _autoAdbCommands.RemoveAll(c => c.Id == id);
            Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Removed {removedCount} record(s) with ID: {id}. Remaining: {_autoAdbCommands.Count}");
            SaveAutoAdbCommands();
        }

        try
        {
            AutoAdbCommandsChanged?.Invoke(this, EventArgs.Empty);
            Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Fired AutoAdbCommandsChanged event");
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Exception in AutoAdbCommandsChanged event: {ex.Message}");
        }

        Logger.Log($"[SettingsManager.RemoveAutoAdbCommand] Finished RemoveAutoAdbCommand for ID: {id}");
    }

    public void ToggleAutoAdbCommand(string id, bool isEnabled)
    {
        Logger.Log($"[SettingsManager.ToggleAutoAdbCommand] Toggling command ID: {id} to isEnabled={isEnabled}");
        AutoAdbCommand? target = null;
        lock (_autoAdbLock)
        {
            target = _autoAdbCommands.FirstOrDefault(c => c.Id == id);
            if (target != null)
            {
                target.IsEnabled = isEnabled;
                SaveAutoAdbCommands();
            }
        }

        if (target != null)
        {
            try
            {
                if (isEnabled)
                {
                    AutoAdbService.Instance.Start(target);
                }
                else
                {
                    AutoAdbService.Instance.Stop(target.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                Logger.Log($"[SettingsManager.ToggleAutoAdbCommand] Error during start/stop: {ex.Message}");
            }
        }
        else
        {
            Logger.Log($"[SettingsManager.ToggleAutoAdbCommand] Target command ID: {id} not found");
        }

        try
        {
            AutoAdbCommandsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }
}