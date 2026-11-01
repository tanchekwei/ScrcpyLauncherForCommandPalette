// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Features;
using ScrcpyLauncherForCommandPalette.Features.Diagnostic;
using ScrcpyLauncherForCommandPalette.Helpers;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class StartAppListPage : DynamicListPage
{
    private readonly IScrcpyFeature _startAppFeature;
    private readonly ScrcpyCommandBuilderService _service;
    private readonly Action? _onUpdated;
    private readonly List<ListItem> _items = [];
    private readonly object _lock = new();
    private bool _appsLoadingStarted;

    public StartAppListPage(
        IScrcpyFeature startAppFeature,
        ScrcpyCommandBuilderService service,
        Action? onUpdated)
    {
        _startAppFeature = startAppFeature;
        _service = service;
        _onUpdated = onUpdated;

        Name = "Start App on Launch";
        Title = "Select App to Start (--start-app)";
        Icon = Classes.Icon.Phone;

        RebuildItems(string.Empty);
    }

    public override IListItem[] GetItems()
    {
        lock (_lock)
        {
            EnsureAppsLoaded();
            if (_items.Count == 0)
            {
                RebuildItems(SearchText ?? string.Empty);
            }
            return [.. _items];
        }
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RebuildItems(newSearch);
        RaiseItemsChanged(_items.Count);
    }

    public override void LoadMore()
    {
        HasMoreItems = false;
    }
    private sealed record CandidateItemEntry(ListItem Item, bool IsSelected);

    private void EnsureAppsLoaded()
    {
        var state = _service.State;
        string serial = state.DeviceSerial;
        if (string.IsNullOrEmpty(serial))
        {
            try
            {
                var devices = AdbHelper.GetConnectedDevices();
                if (devices.Length > 0)
                {
                    serial = devices[0].Serial;
                    state.DeviceSerial = serial;
                }
            }
            catch
            {
            }
        }

        if (state.InstalledApps.Count == 0 && ListApps.TryGetCachedApps(serial, out var cached) && cached.Count > 0)
        {
            state.InstalledApps = cached;
        }

        if (_appsLoadingStarted || state.InstalledApps.Count > 0)
        {
            return;
        }

        _appsLoadingStarted = true;
        state.IsLoadingApps = true;

        Task.Run(() =>
        {
            try
            {
                var apps = ListApps.GetCachedOrFetchApps(serial, forceRefresh: true);
                state.InstalledApps = apps;
            }
            catch (Exception ex)
            {
                Logger.Log($"[StartAppListPage] Error loading apps: {ex.Message}");
            }
            finally
            {
                state.IsLoadingApps = false;
                RebuildItems(SearchText ?? string.Empty);
                RaiseItemsChanged(_items.Count);
            }
        });
    }

    private void RebuildItems(string query)
    {
        lock (_lock)
        {
            _items.Clear();
            var state = _service.State;
            var trimmedQuery = query?.Trim() ?? string.Empty;

            // 1. Current selection / Clear action if an app is selected
            if (!string.IsNullOrWhiteSpace(state.StartApp))
            {
                var matchingApp = state.InstalledApps.FirstOrDefault(a => string.Equals(a.PackageName, state.StartApp, StringComparison.OrdinalIgnoreCase));
                string displayName = matchingApp != null ? $"{matchingApp.AppName} ({state.StartApp})" : state.StartApp;

                var clearCmd = new AnonymousCommand(() =>
                {
                    _service.State.StartApp = string.Empty;
                    _service.RebuildFinalCommand();
                    _onUpdated?.Invoke();
                    new ToastStatusMessage("Cleared start app option.").Show();
                })
                {
                    Name = "Clear Start App",
                    Icon = Classes.Icon.Cancel,
                    Result = CommandResult.GoBack(),
                };

                var clearItem = new ListItem(clearCmd)
                {
                    Title = $"Clear: {displayName}",
                    Subtitle = "Click to remove --start-app option from command",
                    Icon = Classes.Icon.Cancel,
                    Tags = [new Tag("Current Selection"), new Tag("Click: Clear")],
                };
                _items.Add(clearItem);
            }

            // 2. Custom input option if user typed something in search
            if (!string.IsNullOrWhiteSpace(trimmedQuery))
            {
                var customCmd = new AnonymousCommand(() =>
                {
                    _service.State.StartApp = trimmedQuery;
                    _service.RebuildFinalCommand();
                    _onUpdated?.Invoke();
                    new ToastStatusMessage($"Set start app to {trimmedQuery}").Show();
                })
                {
                    Name = $"Use \"{trimmedQuery}\"",
                    Icon = Classes.Icon.Edit,
                    Result = CommandResult.GoBack(),
                };

                var customItem = new ListItem(customCmd)
                {
                    Title = $"Use Custom: \"{trimmedQuery}\"",
                    Subtitle = $"Apply --start-app={trimmedQuery}",
                    Icon = Classes.Icon.Edit,
                    Tags = [new Tag("Custom Package / Query")],
                };
                _items.Add(customItem);
            }

            // 3. Option to enter custom package via dialog/form
            var configureCustomPage = new ScrcpyFeatureConfigurePage(
                _startAppFeature,
                "Custom Package / Query",
                () =>
                {
                    _service.RebuildFinalCommand();
                    _onUpdated?.Invoke();
                    RebuildItems(SearchText ?? string.Empty);
                    RaiseItemsChanged(_items.Count);
                },
                _service);

            var manualItem = new ListItem(configureCustomPage)
            {
                Title = "Enter Custom Package Name / Query...",
                Subtitle = !string.IsNullOrWhiteSpace(state.StartApp)
                    ? $"Current: {state.StartApp} (e.g. org.mozilla.firefox, ?query, or +component)"
                    : "Manually type a package name, ?search_query, or +component",
                Icon = Classes.Icon.Edit,
                Tags = [new Tag("Manual Input")],
            };
            _items.Add(manualItem);

            // 4. Refresh installed apps button
            var refreshCmd = new AnonymousCommand(() =>
            {
                IsLoading = true;
                new ToastStatusMessage("Refreshing apps from device...").Show();
                Task.Run(() =>
                {
                    try
                    {
                        string serial = _service.State.DeviceSerial;
                        var apps = ListApps.GetCachedOrFetchApps(serial, forceRefresh: true);
                        _service.State.InstalledApps = apps;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex);
                    }
                    finally
                    {
                        RebuildItems(SearchText ?? string.Empty);
                        RaiseItemsChanged(_items.Count);
                        new ToastStatusMessage($"Loaded {_service.State.InstalledApps.Count} apps.").Show();
                        IsLoading = false;
                    }
                });
            })
            {
                Name = "Refresh Apps",
                Icon = Classes.Icon.Refresh,
                Result = CommandResult.KeepOpen(),
            };

            var refreshItem = new ListItem(refreshCmd)
            {
                Title = "Refresh Installed Apps from Device (--list-apps)",
                Subtitle = state.InstalledApps.Count > 0
                    ? $"{state.InstalledApps.Count} apps currently loaded. Click to re-scan."
                    : "Click to fetch installed apps from the connected device.",
                Icon = Classes.Icon.Refresh,
                Tags = [new Tag("Action")],
            };
            _items.Add(refreshItem);

            _items.Add(new ListItem(new Separator()));

            // 5. Installed apps list
            if (state.IsLoadingApps)
            {
                var loadingItem = new ListItem(new AnonymousCommand(() => { }) { Result = CommandResult.KeepOpen() })
                {
                    Title = "Loading Installed Apps...",
                    Subtitle = "Querying packages from device via adb / scrcpy --list-apps",
                    Icon = Classes.Icon.Refresh,
                    Tags = [new Tag("Loading")],
                };
                _items.Add(loadingItem);
                return;
            }

            var apps = state.InstalledApps;
            if (apps.Count == 0)
            {
                var emptyItem = new ListItem(refreshCmd)
                {
                    Title = "No Installed Apps Found",
                    Subtitle = "Make sure a device is connected and click here to scan, or use custom package input above.",
                    Icon = Classes.Icon.Info,
                    Tags = [new Tag("Empty")],
                };
                _items.Add(emptyItem);
                return;
            }

            var candidates = new List<CandidateItemEntry>();
            foreach (var app in apps)
            {
                bool isSelected = string.Equals(app.PackageName, state.StartApp, StringComparison.OrdinalIgnoreCase);

                var tags = new List<ITag>();
                if (isSelected)
                {
                    tags.Add(new Tag("Selected"));
                }

                var selectCmd = new AnonymousCommand(() =>
                {
                    _service.State.StartApp = app.PackageName;
                    _service.RebuildFinalCommand();
                    _onUpdated?.Invoke();
                    new ToastStatusMessage($"Selected {app.AppName}").Show();
                })
                {
                    Name = $"Select {app.AppName}",
                    Icon = isSelected ? Classes.Icon.Check : Classes.Icon.Phone,
                    Result = CommandResult.GoBack(),
                };

                var appItem = new ListItem(selectCmd)
                {
                    Title = app.AppName,
                    Subtitle = app.PackageName,
                    Icon = isSelected ? Classes.Icon.Check : Classes.Icon.Phone,
                    Tags = [.. tags],
                    MoreCommands =
                    [
                        new CommandContextItem(new AnonymousCommand(() =>
                        {
                            ScrcpyRunner.CopyToClipboard(app.PackageName);
                        }) { Name = "Copy Package Name", Icon = Classes.Icon.Copy, Result = CommandResult.KeepOpen() }),
                    ],
                };

                candidates.Add(new CandidateItemEntry(appItem, isSelected));
            }

            // 5. Build items list with query:
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Selected features always remain visible; sort matching ones higher if they match query
                var orderedSelected = candidates
                    .Select(c => new
                    {
                        c.Item,
                        Score = ListHelpers.ScoreListItem(query, c.Item)
                    })
                    .OrderByDescending(s => s.Score)
                    .Select(s => s.Item)
                    .ToList();


                _items.AddRange(orderedSelected);
                if (orderedSelected.Count > 0)
                {
                    _items.Add(new ListItem(new Separator()));
                }
            }
            else
            {
                var selectedList = candidates.Select(c => c.Item).ToList();

                _items.AddRange(selectedList);
                if (selectedList.Count > 0)
                {
                    _items.Add(new ListItem(new Separator()));
                }
            }
        }
    }
}