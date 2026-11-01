// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;
using ScrcpyLauncherForCommandPalette.Features;
using ScrcpyLauncherForCommandPalette.Helpers;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyCommandBuilderPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly DeviceInfo? _targetDevice;
    private readonly ScrcpyFavoriteCommand? _existingFavorite;
    private readonly ScrcpyCommandBuilderService _service;
    private readonly List<ListItem> _items = [];
    private readonly object _lock = new();

    private sealed record CandidateItemEntry(ListItem Item, bool IsSelected, string Category);

    public ScrcpyCommandBuilderService Service => _service;

    public ScrcpyCommandBuilderPage(
        DeviceInfo? targetDevice = null,
        ScrcpyFavoriteCommand? existingFavorite = null,
        SettingsManager? settingsManager = null,
        ScrcpyCommandBuilderService? service = null)
    {
        _targetDevice = targetDevice;
        _existingFavorite = existingFavorite;
        _settingsManager = settingsManager ?? SettingsManager.Instance ?? ScrcpyRunner.SettingsManager ?? new SettingsManager();

        if (existingFavorite != null)
        {
            Name = $"Edit: {existingFavorite.Name}";
            Title = $"Edit Preset: {existingFavorite.Name}";
            Icon = Classes.Icon.Edit;
        }
        else if (targetDevice != null)
        {
            Name = $"{targetDevice.Model} Command Builder";
            Title = $"Command Builder ({targetDevice.Model})";
            Icon = Classes.Icon.Setting;
        }
        else
        {
            Name = "Command Builder";
            Title = "Command Builder";
            Icon = Classes.Icon.Setting;
        }

        // Initialize state in this page's own service instance
        _service = service ?? new ScrcpyCommandBuilderService();
        _service.Initialize(_targetDevice, _existingFavorite);

        var filters = new SearchFilters();
        filters.PropChanged += Filters_PropChanged;
        Filters = filters;

        RebuildItems(string.Empty);
    }

    private void Filters_PropChanged(object sender, IPropChangedEventArgs args)
    {
        UpdateSearchText(string.Empty, SearchText);
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

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RebuildItems(newSearch);
        RaiseItemsChanged(_items.Count);
    }

    public override void LoadMore()
    {
        HasMoreItems = false;
    }

    private void RebuildItems(string query)
    {
        lock (_lock)
        {
            _items.Clear();

            var filterId = Filters?.CurrentFilterId ?? string.Empty;
            bool isFilterAll = string.IsNullOrEmpty(filterId) || string.Equals(filterId, SearchFilters.FilterAll, StringComparison.OrdinalIgnoreCase);

            var service = _service;
            var state = service.State;
            string commandLine = string.IsNullOrWhiteSpace(state.FinalCommand) ? "scrcpy" : state.FinalCommand;

            // 1. Preview item ALWAYS on top
            var editCommandLinePage = CreateEditCommandLinePage(commandLine);
            var args = ScrcpyCommandBuilderState.ExtractArguments(commandLine);
            var previewItem = new ListItem(editCommandLinePage)
            {
                Title = commandLine,
                Subtitle = "Current scrcpy command line. Click to edit command line or use More Commands to run/copy.",
                Icon = Classes.Icon.Terminal,
                Tags = [new Tag("Command Preview"), new Tag("Click: Edit")],
                MoreCommands =
                [
                    new CommandContextItem(new RunDirectlyScrcpyCommand(args)),
                    new CommandContextItem(new RunInConsoleScrcpyCommand(args)),
                    new CommandContextItem(new CopyScrcpyCommand(args)),
                    new CommandContextItem(editCommandLinePage),
                    new CommandContextItem(CreateSaveFavoritePage(commandLine)),
                    new CommandContextItem(new AnonymousCommand(() =>
                    {
                        service.Reset(_targetDevice);
                        RebuildItems(SearchText ?? string.Empty);
                        RaiseItemsChanged(_items.Count);
                        new ToastStatusMessage("Reset options to default.").Show();
                    }) { Name = "Reset to Default", Icon = Classes.Icon.Refresh, Result = CommandResult.KeepOpen() }),
                ]
            };
            _items.Add(previewItem);

            // 2. Collect candidate items (Action items + Feature items)
            var candidates = new List<CandidateItemEntry>();

            // Run Shortcut Item (General action)
            var runItem = new ListItem(new CommandContextItem(new RunDirectlyScrcpyCommand(args)))
            {
                Title = "Launch scrcpy with Current Options",
                Subtitle = $"Execute: {commandLine}",
                Icon = Classes.Icon.Play,
                Tags = [new Tag("Action"), new Tag("Run")],
                MoreCommands =
                [
                    new CommandContextItem(new RunInConsoleScrcpyCommand(args)),
                ]
            };
            _items.Add(runItem);

            // Preset / AutoAdb / Reset actions (General action)
            var saveItem = new ListItem(CreateSaveFavoritePage(commandLine))
            {
                Title = _existingFavorite != null ? $"Update Preset: {_existingFavorite.Name}" : "Save Current Configuration to Favorites",
                Subtitle = !string.IsNullOrWhiteSpace(state.FavoriteName) ? $"Preset Name: {state.FavoriteName}" : "Click to save as a reusable preset",
                Icon = _existingFavorite != null ? Classes.Icon.Edit : Classes.Icon.Pinned,
                Tags = [new Tag("Preset"), new Tag("Favorites")],
                MoreCommands =
                [
                    new CommandContextItem(new AnonymousCommand(() =>
                    {
                        AddToAutoAdb(commandLine);
                    }) { Name = "Add to AutoAdb", Icon = Classes.Icon.Flow, Result = CommandResult.KeepOpen() }),
                    new CommandContextItem(new AnonymousCommand(() =>
                    {
                        service.Reset(_targetDevice);
                        RebuildItems(SearchText ?? string.Empty);
                        RaiseItemsChanged(_items.Count);
                        new ToastStatusMessage("Reset options to default.").Show();
                    }) { Name = "Reset Options", Icon = Classes.Icon.Refresh, Result = CommandResult.KeepOpen() })
                ]
            };
            _items.Add(saveItem);
            _items.Add(new ListItem(new Separator()));

            // 3. Feature Items from ScrcpyFeatureRegistry
            foreach (var feature in ScrcpyFeatureRegistry.Features)
            {
                var sampleElement = feature.CreateCardElement(state);
                if (sampleElement == null && feature is not IScrcpyCustomPageFeature)
                {
                    continue;
                }

                string featureType = (sampleElement?["type"]?.ToString() ?? string.Empty).ToLowerInvariant();
                string title = sampleElement != null ? GetFeatureTitle(feature, sampleElement) : (feature.Commands.Count > 0 ? feature.Commands[0].Title : feature.GetType().Name);
                string desc = sampleElement != null ? GetFeatureDescription(feature, sampleElement) : (feature.Commands.Count > 0 ? feature.Commands[0].Description : $"Configure {feature.Category} option");

                bool isToggle = featureType == "input.toggle";
                bool isSelected = IsFeatureSelected(feature, state, sampleElement);
                string selectedDisplayValue = GetFeatureSelectedDisplayValue(feature, state, sampleElement);

                var tags = new List<ITag>
                {
                    new Tag(feature.Category)
                };

                if (isSelected)
                {
                    tags.Add(new Tag("Active / Selected"));
                    if (!string.IsNullOrEmpty(selectedDisplayValue) && !isToggle)
                    {
                        tags.Add(new Tag(selectedDisplayValue));
                    }
                }

                if (isToggle)
                {
                    string toggleLabel = isSelected ? $"Disable {title}" : $"Enable {title}";

                    var toggleCommand = new AnonymousCommand(() =>
                    {
                        ToggleFeature(feature, state, sampleElement!);
                        service.RebuildFinalCommand();
                        RebuildItems(SearchText ?? string.Empty);
                        RaiseItemsChanged(_items.Count);
                    })
                    {
                        Name = toggleLabel,
                        Icon = isSelected ? Classes.Icon.CheckboxComposite : Classes.Icon.Checkbox,
                        Result = CommandResult.KeepOpen()
                    };

                    var moreCommands = new List<ICommandContextItem>
                    {
                        new CommandContextItem(new AnonymousCommand(() =>
                            {
                                feature.ResetState(state);
                                service.RebuildFinalCommand();
                                RebuildItems(SearchText ?? string.Empty);
                                RaiseItemsChanged(_items.Count);
                            }) { Name = "Reset this Option", Icon = Classes.Icon.Refresh, Result = CommandResult.KeepOpen() }),
                    };

                    if (feature.Commands.Count > 0 && !string.IsNullOrWhiteSpace(feature.Commands[0].DocumentationUrl))
                    {
                        moreCommands.Add(new CommandContextItem(new OpenUrlDocCommand("Documentation", feature.Commands[0].DocumentationUrl!)));
                    }

                    var listItem = new ListItem(toggleCommand)
                    {
                        Title = $"{title}",
                        Subtitle = desc,
                        Icon = isSelected ? Classes.Icon.CheckboxComposite : Classes.Icon.Checkbox,
                        Tags = [.. tags],
                        MoreCommands = [.. moreCommands],
                    };
                    candidates.Add(new CandidateItemEntry(listItem, isSelected, feature.Category));
                }
                else
                {
                    string subtitle = !string.IsNullOrEmpty(selectedDisplayValue)
                        ? $"{selectedDisplayValue} | {desc}"
                        : desc;

                    IPage pageToOpen;
                    if (feature is IScrcpyCustomPageFeature customPageFeature)
                    {
                        pageToOpen = customPageFeature.CreateCustomPage(
                            _service,
                            () =>
                            {
                                service.RebuildFinalCommand();
                                RebuildItems(SearchText ?? string.Empty);
                                RaiseItemsChanged(_items.Count);
                            });
                    }
                    else
                    {
                        pageToOpen = new ScrcpyFeatureConfigurePage(
                            feature,
                            title,
                            () =>
                            {
                                service.RebuildFinalCommand();
                                RebuildItems(SearchText ?? string.Empty);
                                RaiseItemsChanged(_items.Count);
                            },
                            _service);
                    }

                    var moreCommands = new List<ICommandContextItem>();
                    if (isSelected)
                    {
                        moreCommands.Add(new CommandContextItem(new AnonymousCommand(() =>
                        {
                            feature.ResetState(state);
                            service.RebuildFinalCommand();
                            RebuildItems(SearchText ?? string.Empty);
                            RaiseItemsChanged(_items.Count);
                        })
                        { Name = "Reset this Option", Icon = Classes.Icon.Refresh, Result = CommandResult.KeepOpen() }));
                    }

                    foreach (var cmd in feature.Commands)
                    {
                        if (!string.IsNullOrWhiteSpace(cmd.Arguments))
                        {
                            moreCommands.Add(new CommandContextItem(new AnonymousCommand(() =>
                            {
                                state.LoadFromArguments(cmd.Arguments);
                                service.RebuildFinalCommand();
                                RebuildItems(SearchText ?? string.Empty);
                                RaiseItemsChanged(_items.Count);
                            })
                            {
                                Name = $"Apply: {cmd.Title} ({cmd.Arguments})",
                                Icon = Classes.Icon.Play,
                                Result = CommandResult.KeepOpen()
                            }));
                        }
                    }

                    if (feature.Commands.Count > 0 && !string.IsNullOrWhiteSpace(feature.Commands[0].DocumentationUrl))
                    {
                        moreCommands.Add(new CommandContextItem(new OpenUrlDocCommand("Documentation", feature.Commands[0].DocumentationUrl!)));
                    }

                    if (feature is IScrcpyCustomPageFeature)
                    {
                        moreCommands.Add(new CommandContextItem(new ScrcpyFeatureConfigurePage(
                            feature,
                            $"{title} (Custom Input)",
                            () =>
                            {
                                service.RebuildFinalCommand();
                                RebuildItems(SearchText ?? string.Empty);
                                RaiseItemsChanged(_items.Count);
                            },
                            _service)));
                    }

                    var listItem = new ListItem(pageToOpen)
                    {
                        Title = title,
                        Subtitle = subtitle,
                        Icon = isSelected ? Classes.Icon.Setting : feature.Category == "Diagnostic" ? Classes.Icon.Terminal : Classes.Icon.Edit,
                        Tags = [.. tags],
                        MoreCommands = [.. moreCommands],
                    };

                    candidates.Add(new CandidateItemEntry(listItem, isSelected, feature.Category));
                }
            }

            // 4. Partition candidates into Selected and NonSelected
            // Selected features ALWAYS show on the list (like the preview item),
            // while only non-selected features are filtered by the category filter and query.
            var selectedCandidates = candidates.Where(c => c.IsSelected).ToList();
            var nonSelectedCandidates = candidates.Where(c => !c.IsSelected);

            if (!isFilterAll)
            {
                nonSelectedCandidates = nonSelectedCandidates.Where(c => string.Equals(c.Category, filterId, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Build items list with query:
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Selected features always remain visible; sort matching ones higher if they match query
                var orderedSelected = selectedCandidates
                    .Select(c => new
                    {
                        c.Item,
                        Score = ListHelpers.ScoreListItem(query, c.Item)
                    })
                    .OrderByDescending(s => s.Score)
                    .Select(s => s.Item)
                    .ToList();

                // Non-selected features must match query (score > 0)
                var scoredNonSelected = nonSelectedCandidates
                    .Select(c => new
                    {
                        c.Item,
                        Score = ListHelpers.ScoreListItem(query, c.Item)
                    })
                    .Where(s => s.Score > 0)
                    .OrderByDescending(s => s.Score)
                    .Select(s => s.Item)
                    .ToList();

                _items.AddRange(orderedSelected);
                if (orderedSelected.Count > 0 && scoredNonSelected.Count > 0)
                {
                    _items.Add(new ListItem(new Separator()));
                }
                _items.AddRange(scoredNonSelected);
            }
            else
            {
                var selectedList = selectedCandidates.Select(c => c.Item).ToList();
                var nonSelectedList = nonSelectedCandidates.Select(c => c.Item).ToList();

                _items.AddRange(selectedList);
                if (selectedList.Count > 0 && nonSelectedList.Count > 0)
                {
                    _items.Add(new ListItem(new Separator()));
                }
                _items.AddRange(nonSelectedList);
            }
        }
    }

    private static string GetFeatureTitle(IScrcpyFeature feature, JsonObject element)
    {
        if (element.TryGetPropertyValue("title", out var titleNode) && titleNode != null)
        {
            var str = titleNode.ToString();
            if (!string.IsNullOrWhiteSpace(str)) return str;
        }

        if (element.TryGetPropertyValue("label", out var labelNode) && labelNode != null)
        {
            var str = labelNode.ToString();
            if (!string.IsNullOrWhiteSpace(str)) return str;
        }

        if (element.TryGetPropertyValue("items", out var itemsNode) && itemsNode is JsonArray itemsArr)
        {
            foreach (var child in itemsArr)
            {
                if (child is JsonObject childObj)
                {
                    if (childObj.TryGetPropertyValue("title", out var cTitle) && cTitle != null) return cTitle.ToString();
                    if (childObj.TryGetPropertyValue("label", out var cLabel) && cLabel != null) return cLabel.ToString();
                }
            }
        }

        if (feature.Commands.Count > 0)
        {
            return feature.Commands[0].Title;
        }

        return feature.GetType().Name;
    }

    private static string GetFeatureDescription(IScrcpyFeature feature, JsonObject element)
    {
        if (element.TryGetPropertyValue("placeholder", out var phNode) && phNode != null)
        {
            var ph = phNode.ToString();
            if (!string.IsNullOrWhiteSpace(ph)) return ph;
        }

        if (feature.Commands.Count > 0 && !string.IsNullOrWhiteSpace(feature.Commands[0].Description))
        {
            return feature.Commands[0].Description;
        }

        return $"Configure {feature.Category} option";
    }

    private static bool IsFeatureSelected(IScrcpyFeature feature, ScrcpyCommandBuilderState state, JsonObject? element)
    {
        var tempArgs = new List<string>();
        feature.BuildArguments(state, tempArgs);
        return tempArgs.Count > 0;
    }

    private static string GetFeatureSelectedDisplayValue(IScrcpyFeature feature, ScrcpyCommandBuilderState state, JsonObject? element)
    {
        var tempArgs = new List<string>();
        feature.BuildArguments(state, tempArgs);
        if (tempArgs.Count > 0)
        {
            return string.Join(" ", tempArgs);
        }
        return string.Empty;
    }

    private static void ToggleFeature(IScrcpyFeature feature, ScrcpyCommandBuilderState state, JsonObject element)
    {
        string? propId = element["id"]?.ToString();
        if (string.IsNullOrEmpty(propId))
        {
            return;
        }

        // Check if currently selected
        bool currentlySelected = IsFeatureSelected(feature, state, element);
        bool targetValue = !currentlySelected;

        var input = new JsonObject
        {
            [propId] = targetValue.ToString().ToLowerInvariant()
        };

        feature.UpdateState(state, input);
    }

    private ScrcpyEditCommandLinePage CreateEditCommandLinePage(string commandLine)
    {
        return new ScrcpyEditCommandLinePage(
            commandLine,
            _targetDevice,
            () =>
            {
                RebuildItems(SearchText ?? string.Empty);
                RaiseItemsChanged(_items.Count);
            },
            _service);
    }

    private ScrcpySaveFavoritePage CreateSaveFavoritePage(string commandLine)
    {
        return new ScrcpySaveFavoritePage(
            commandLine,
            _targetDevice,
            _existingFavorite,
            _settingsManager,
            () =>
            {
                if (_existingFavorite != null)
                {
                    Name = $"Edit: {_existingFavorite.Name}";
                    Title = $"Edit Preset: {_existingFavorite.Name}";
                }
                RebuildItems(SearchText ?? string.Empty);
                RaiseItemsChanged(_items.Count);
            },
            _service);
    }

    private void AddToAutoAdb(string commandLine)
    {
        var state = _service.State;
        string args = ScrcpyCommandBuilderState.ExtractArguments(commandLine);
        string name = !string.IsNullOrWhiteSpace(state.FavoriteName)
            ? state.FavoriteName.Trim()
            : $"AutoAdb ({DateTime.Now:MMM dd, HH:mm})";

        var autoCmd = new AutoAdbCommand
        {
            Name = name,
            ScrcpyArguments = args,
            IsEnabled = true,
        };

        var sm = SettingsManager.Instance ?? ScrcpyRunner.SettingsManager ?? _settingsManager;
        if (sm != null)
        {
            sm.AddAutoAdbCommand(autoCmd);
            new ToastStatusMessage($"Added '{name}' to AutoAdb!").Show();
        }
        else
        {
            new ToastStatusMessage("Settings not initialized.").Show();
        }
    }
}