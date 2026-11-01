// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;
using ScrcpyLauncherForCommandPalette.Helpers;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyLauncherPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly List<ListItem> _visibleItems = [];
    private readonly object _lock = new();

    private DeviceInfo[] _connectedDevices = [];
    private bool _isRefreshingDevices;
    private readonly ScrcpyCommandListPage _allCommandsPage = new(null);
    private readonly ScrcpyFavoriteCommandsPage _favPage;
    private readonly ScrcpyAutoAdbPage _autoAdbPage;

    private int _lastFavCount = -1;
    private int _lastAutoAdbCount = -1;
    private int _lastRunningAutoAdbCount = -1;

    public ScrcpyLauncherPage(SettingsManager settingsManager)
    {
        Name = Constant.ScrcpyLauncherDisplayName;
        Icon = Classes.Icon.Extension;
        // Id = $"{Package.Current.Id.Name}.{nameof(ScrcpyLauncherPage)}";

        _settingsManager = settingsManager;
        _favPage = new ScrcpyFavoriteCommandsPage(_settingsManager);
        _autoAdbPage = new ScrcpyAutoAdbPage(_settingsManager);

        _settingsManager.FavoritesChanged += OnSettingsOrStatusChanged;
        _settingsManager.AutoAdbCommandsChanged += OnSettingsOrStatusChanged;
        AutoAdbService.Instance.StatusChanged += OnSettingsOrStatusChanged;

        if (SettingsManager.Instance != null && SettingsManager.Instance != _settingsManager)
        {
            SettingsManager.Instance.FavoritesChanged += OnSettingsOrStatusChanged;
            SettingsManager.Instance.AutoAdbCommandsChanged += OnSettingsOrStatusChanged;
        }

        _ = RefreshDevicesAsync();
        RebuildVisibleItems(string.Empty);
    }

    private void OnSettingsOrStatusChanged(object? sender, EventArgs e)
    {
        try
        {
            RebuildVisibleItems(SearchText ?? string.Empty);
            RaiseItemsChanged(_visibleItems.Count);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }

    public async Task RefreshDevicesAsync(bool showToast = false)
    {
        if (_isRefreshingDevices) return;
        _isRefreshingDevices = true;

        try
        {
            var devices = await Task.Run(AdbHelper.GetConnectedDevices);
            lock (_lock)
            {
                _connectedDevices = devices;
            }
            RebuildVisibleItems(SearchText);
            RaiseItemsChanged(_visibleItems.Count);

            if (showToast)
            {
                string message = devices.Length > 0
                    ? $"Found {devices.Length} connected device{(devices.Length > 1 ? "s" : "")}."
                    : "No Android devices detected.";
                new ToastStatusMessage(message).Show();
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            if (showToast)
            {
                new ToastStatusMessage($"Error refreshing devices: {ex.Message}").Show();
            }
        }
        finally
        {
            _isRefreshingDevices = false;
        }
    }

    public void StartRefresh()
    {
        _ = RefreshDevicesAsync(showToast: false);
    }

    public override IListItem[] GetItems()
    {
        lock (_lock)
        {
            int curFavCount = _settingsManager.Favorites.Count;
            int curAutoAdbCount = _settingsManager.AutoAdbCommands.Count;
            int curRunningAutoAdb = AutoAdbService.Instance.RunningCount;

            if (_visibleItems.Count == 0 ||
                curFavCount != _lastFavCount ||
                curAutoAdbCount != _lastAutoAdbCount ||
                curRunningAutoAdb != _lastRunningAutoAdbCount)
            {
                RebuildVisibleItems(SearchText ?? string.Empty);
            }
            return [.. _visibleItems];
        }
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RebuildVisibleItems(newSearch);
        RaiseItemsChanged(_visibleItems.Count);
    }

    private void RebuildVisibleItems(string query)
    {
        lock (_lock)
        {
            _visibleItems.Clear();
            var trimmedQuery = query?.Trim() ?? string.Empty;

            _lastFavCount = _settingsManager.Favorites.Count;
            _lastAutoAdbCount = _settingsManager.AutoAdbCommands.Count;
            _lastRunningAutoAdbCount = AutoAdbService.Instance.RunningCount;

            // Custom CLI command running if user typed a query
            if (!string.IsNullOrWhiteSpace(trimmedQuery))
            {
                string customArgs;
                string customFullCmd;

                if (trimmedQuery.StartsWith("scrcpy", StringComparison.OrdinalIgnoreCase))
                {
                    customFullCmd = trimmedQuery;
                    customArgs = trimmedQuery.Length > 6 ? trimmedQuery[6..].Trim() : string.Empty;
                }
                else
                {
                    customArgs = trimmedQuery;
                    customFullCmd = $"scrcpy {trimmedQuery}";
                }

                // If exactly 1 device exists and user didn't specify -s, auto-target that device
                if (_connectedDevices.Length == 1 && !customArgs.Contains("-s "))
                {
                    customArgs = string.IsNullOrEmpty(customArgs)
                        ? $"-s {_connectedDevices[0].Serial}"
                        : $"-s {_connectedDevices[0].Serial} {customArgs}";
                    customFullCmd = $"scrcpy {customArgs}";
                }

                var customRunCmd = new RunScrcpyCommand($"Run: {customFullCmd}", customArgs, Classes.Icon.Terminal);
                var customItem = new ListItem(customRunCmd)
                {
                    Title = $"Run: {customFullCmd}",
                    Subtitle = "Execute custom scrcpy command line",
                    Icon = Classes.Icon.Terminal,
                    Tags = [new Tag("Custom CLI")],
                    MoreCommands = [
                        new CommandContextItem(new RunInConsoleScrcpyCommand(customArgs)),
                        new CommandContextItem(new RunDirectlyScrcpyCommand(customArgs)),
                        new CommandContextItem(new CopyScrcpyCommand(customFullCmd)),
                    ],
                };

                _visibleItems.Add(customItem);
            }

            var refreshCommand = new AnonymousCommand(() => _ = RefreshDevicesAsync(showToast: true))
            {
                Name = "Refresh",
                Icon = Classes.Icon.Refresh,
                Result = CommandResult.KeepOpen(),
            };

            var refreshItem = new ListItem(refreshCommand)
            {
                Title = "Refresh Connected Devices",
                Subtitle = _connectedDevices.Length > 0
                    ? $"Scan for newly connected or disconnected devices ({_connectedDevices.Length} currently detected)"
                    : "No devices detected. Connect an Android device via USB/Wi-Fi and click to scan",
                Icon = Classes.Icon.Refresh,
                Tags = [new Tag("Action"), new Tag("Refresh")],
            };

            if (string.IsNullOrWhiteSpace(trimmedQuery) ||
                refreshItem.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                refreshItem.Subtitle.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
            {
                _visibleItems.Add(refreshItem);
            }


            // 1. Device Navigation items (if devices connected)
            if (_connectedDevices.Length > 0)
            {
                foreach (var device in _connectedDevices)
                {
                    var devBuilderPage = new ScrcpyCommandBuilderPage(device);
                    var devBuilderItem = new ListItem(devBuilderPage)
                    {
                        Title = $"Command Builder ({device.Model})",
                        Subtitle = $"Build, combine and customize scrcpy CLI command for {device.Model}",
                        Icon = Classes.Icon.Setting,
                        Tags = [new Tag("Builder"), new Tag(device.Model)],
                        MoreCommands = [new CommandContextItem(refreshCommand)],
                    };

                    if (string.IsNullOrWhiteSpace(trimmedQuery) ||
                        devBuilderItem.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                        devBuilderItem.Subtitle.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                    {
                        _visibleItems.Add(devBuilderItem);
                    }
                }
            }
            else
            {
                var allCommandsBuilderPage = new ScrcpyCommandBuilderPage(_connectedDevices.Length == 1 ? _connectedDevices[0] : null);
                var allCommandsBuilderItem = new ListItem(allCommandsBuilderPage)
                {
                    Title = "Command Builder",
                    Subtitle = "Build and combine scrcpy CLI flags (Screen, Audio, Camera, Recording) into a custom command",
                    Icon = Classes.Icon.Setting,
                    Tags = [new Tag("Builder"), new Tag("Interactive")],
                };

                if (string.IsNullOrWhiteSpace(trimmedQuery) ||
                    allCommandsBuilderItem.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                    allCommandsBuilderItem.Subtitle.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
                {
                    _visibleItems.Add(allCommandsBuilderItem);
                }
            }

            var favItem = new ListItem(_favPage)
            {
                Title = "Favorite Commands",
                Subtitle = $"Browse and run your saved custom scrcpy presets ({_settingsManager.Favorites.Count} saved)",
                Icon = Classes.Icon.Pinned,
                Tags = [new Tag("Favorites"), new Tag("Presets")],
            };

            if (string.IsNullOrWhiteSpace(trimmedQuery) ||
                favItem.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                favItem.Subtitle.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
            {
                _visibleItems.Add(favItem);
            }

            int runningAutoAdb = AutoAdbService.Instance.RunningCount;
            int totalAutoAdb = _settingsManager.AutoAdbCommands.Count;
            var autoAdbItem = new ListItem(_autoAdbPage)
            {
                Title = "AutoAdb Commands",
                Subtitle = runningAutoAdb > 0
                    ? $"Auto-run scrcpy on device connect (🟢 {runningAutoAdb} of {totalAutoAdb} active)"
                    : $"Auto-run scrcpy on device connect ({totalAutoAdb} configured)",
                Icon = Classes.Icon.Flow,
                Tags = [new Tag("AutoAdb"), new Tag("Automation")],
            };

            if (string.IsNullOrWhiteSpace(trimmedQuery) ||
                autoAdbItem.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                autoAdbItem.Subtitle.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
            {
                _visibleItems.Add(autoAdbItem);
            }

            _visibleItems.Add(StaticHelpItems.OpenSettingsFolder);
            _visibleItems.Add(StaticHelpItems.SettingsItem);
        }
    }

    public override void LoadMore()
    {
        HasMoreItems = false;
    }
}