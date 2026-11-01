// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyAutoAdbPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly List<ListItem> _items = [];
    private readonly object _lock = new();

    public ScrcpyAutoAdbPage(SettingsManager? settingsManager = null)
    {
        _settingsManager = settingsManager ?? SettingsManager.Instance ?? new SettingsManager();

        Name = "AutoAdb Commands";
        Title = "AutoAdb Commands";
        Icon = Classes.Icon.Flow;
        // Id = "ScrcpyAutoAdbPage";

        _settingsManager.AutoAdbCommandsChanged += OnStateChanged;
        AutoAdbService.Instance.StatusChanged += OnStateChanged;

        RebuildItems(string.Empty);
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        try
        {
            Logger.Log("[ScrcpyAutoAdbPage.OnStateChanged] AutoAdb state changed, rebuilding list items...");
            RebuildItems(SearchText ?? string.Empty);
            RaiseItemsChanged(_items.Count);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[ScrcpyAutoAdbPage.OnStateChanged] Error: {ex.Message}");
        }
    }

    public override IListItem[] GetItems()
    {
        lock (_lock)
        {
            if (_items.Count == 0)
            {
                RebuildItems(SearchText ?? string.Empty);
            }
            return [.. _items];
        }
    }

    public void Refresh()
    {
        try
        {
            Logger.Log("[ScrcpyAutoAdbPage.Refresh] Refreshing AutoAdb status...");
            RebuildItems(SearchText ?? string.Empty);
            RaiseItemsChanged(_items.Count);
            new ToastStatusMessage("AutoAdb status refreshed.").Show();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[ScrcpyAutoAdbPage.Refresh] Error: {ex.Message}");
        }
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        try
        {
            RebuildItems(newSearch);
            RaiseItemsChanged(_items.Count);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }

    private void RebuildItems(string query)
    {
        lock (_lock)
        {
            _items.Clear();
            var commands = _settingsManager.AutoAdbCommands;
            int runningCount = AutoAdbService.Instance.RunningCount;

            Logger.Log($"[ScrcpyAutoAdbPage.RebuildItems] Rebuilding items with {commands.Count} command(s) and {runningCount} running process(es)");

            var refreshCommand = new AnonymousCommand(Refresh)
            {
                Name = "Refresh",
                Icon = Classes.Icon.Refresh,
                Result = CommandResult.KeepOpen(),
            };

            // 1. Status overview item
            var statusItem = new ListItem(new NoOpCommand()
            {
            })
            {
                Title = runningCount > 0
                    ? $"🟢 AutoAdb: {runningCount} Active Watcher{(runningCount > 1 ? "s" : "")}"
                    : "⚪ AutoAdb: Idle (No Active Watchers)",
                Subtitle = "Executes scrcpy automatically whenever an Android device connects to adb (Click to refresh)",
                Tags = [new Tag(runningCount > 0 ? "Active" : "Idle"), new Tag("Background")],
            };
            _items.Add(statusItem);

            // 2. Refresh action item
            var refreshItem = new ListItem(refreshCommand)
            {
                Title = "Refresh Status",
                Subtitle = "Refresh and check current status and process IDs of AutoAdb watchers",
                Icon = Classes.Icon.Refresh,
                Tags = [new Tag("Action"), new Tag("Refresh")],
            };
            _items.Add(refreshItem);

            // 3. Existing AutoAdb commands
            if (commands.Count > 0)
            {
                foreach (var cmd in commands)
                {
                    bool isRunning = AutoAdbService.Instance.IsRunning(cmd.Id);
                    int? processId = isRunning ? AutoAdbService.Instance.GetProcessId(cmd.Id) : null;

                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        bool match = cmd.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                     cmd.ScrcpyArguments.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                     cmd.FullCommand.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                     (processId.HasValue && processId.Value.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.OrdinalIgnoreCase));
                        if (!match) continue;
                    }

                    var toggleCmd = new ToggleAutoAdbStatusCommand(cmd.Id, !isRunning, cmd.Name);

                    var outputPage = new ScrcpyAutoAdbOutputPage(cmd);
                    var outputCommandItem = new CommandContextItem(outputPage);

                    var moreCommands = new List<ICommandContextItem>
                    {
                        new CommandContextItem(toggleCmd),
                        new CommandContextItem(new RunScrcpyCommand($"Run scrcpy ({cmd.Name})", cmd.FormattedArguments, Classes.Icon.Phone)),
                        new CommandContextItem(new RunInConsoleScrcpyCommand(cmd.FormattedArguments)),
                        new CommandContextItem(new CopyScrcpyCommand(cmd.FullCommand)),
                        new CommandContextItem(refreshCommand),
                        new CommandContextItem(new RemoveAutoAdbItemCommand(cmd.Id, cmd.Name)),
                    };

                    string statusSubtitle = isRunning
                        ? (processId.HasValue ? $"Running in background (PID: {processId.Value})" : "Running in background")
                        : "Stopped";

                    var tags = new List<Tag>
                    {
                        new(isRunning ? "Running" : "Stopped"),
                    };
                    if (isRunning && processId.HasValue)
                    {
                        tags.Add(new Tag($"PID: {processId.Value}"));
                    }
                    tags.Add(new Tag("AutoAdb"));

                    var item = new ListItem(outputCommandItem)
                    {
                        Title = (isRunning ? "🟢 " : "⚪ ") + cmd.Name,
                        Subtitle = $"{cmd.FullCommand}  —  {statusSubtitle}",
                        Icon = isRunning ? Classes.Icon.Flow : Classes.Icon.Cancel,
                        Tags = [.. tags],
                        MoreCommands = [.. moreCommands],
                    };

                    _items.Add(item);
                }
            }

            // 4. Add from Favorites section
            var favorites = _settingsManager.Favorites;
            var availableFavorites = favorites
                .Where(f => !commands.Any(c => string.Equals(c.Name, f.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (availableFavorites.Count > 0)
            {
                foreach (var fav in availableFavorites)
                {
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        bool match = fav.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                     fav.Arguments.Contains(query, StringComparison.OrdinalIgnoreCase);
                        if (!match) continue;
                    }

                    var addCmd = new AddToAutoAdbFromFavoriteCommand(fav);
                    var favItem = new ListItem(addCmd)
                    {
                        Title = $"+ Add to AutoAdb: {fav.Name}",
                        Subtitle = $"Use favorite preset '{fav.Name}' ({fav.FullCommand}) as AutoAdb watcher",
                        Icon = Classes.Icon.Pinned,
                        Tags = [new Tag("Favorite Preset"), new Tag("Add to AutoAdb")],
                    };
                    _items.Add(favItem);
                }
            }

            // 5. Action to build a new command
            var builderItem = new ListItem(new ScrcpyCommandBuilderPage(null))
            {
                Title = "Create New Preset in Command Builder",
                Subtitle = "Configure options and click 'Add to AutoAdb' or 'Save to Favorites'",
                Icon = Classes.Icon.Setting,
                Tags = [new Tag("Builder")],
            };
            _items.Add(builderItem);
        }
    }

    public override void LoadMore()
    {
        HasMoreItems = false;
    }
}