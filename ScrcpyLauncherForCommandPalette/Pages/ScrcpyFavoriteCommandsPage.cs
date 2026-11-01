// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyFavoriteCommandsPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;
    private readonly List<ListItem> _items = [];
    private readonly object _lock = new();
    private ScrcpyAutoAdbPage? _autoAdbPage;

    public ScrcpyFavoriteCommandsPage(SettingsManager? settingsManager = null)
    {
        _settingsManager = settingsManager ?? SettingsManager.Instance ?? new SettingsManager();

        Name = "Favorite Commands";
        Title = "Favorite Commands";
        Icon = Classes.Icon.Pinned;
        // Id = "ScrcpyFavoriteCommandsPage";

        _settingsManager.FavoritesChanged += (s, e) =>
        {
            RebuildItems(SearchText ?? string.Empty);
            RaiseItemsChanged(_items.Count);
        };

        RebuildItems(string.Empty);
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

    private void RebuildItems(string query)
    {
        lock (_lock)
        {
            _items.Clear();
            var favorites = _settingsManager.Favorites;

            // Shortcut to AutoAdb page (reuse single cached instance)
            _autoAdbPage ??= new ScrcpyAutoAdbPage(_settingsManager);
            var autoAdbShortcut = new ListItem(_autoAdbPage)
            {
                Title = "AutoAdb Background Watchers",
                Subtitle = "View and manage automatic scrcpy execution on device connect",
                Icon = Classes.Icon.Flow,
                Tags = [new Tag("AutoAdb"), new Tag("Automation")],
            };
            _items.Add(autoAdbShortcut);

            if (favorites.Count == 0)
            {
                var emptyItem = new ListItem(new ScrcpyCommandBuilderPage(null))
                {
                    Title = "No Favorite Presets Saved",
                    Subtitle = "Click here to open Command Builder and configure your first combination preset",
                    Icon = Classes.Icon.Info,
                    Tags = [new Tag("Favorites"), new Tag("Empty")],
                };
                _items.Add(emptyItem);
                return;
            }

            var favoriteItems = FavoriteCommandsService.Instance.GetCachedItems(query);
            foreach (var item in favoriteItems)
            {
                _items.Add(item);
            }

            // Action to build a new command / preset
            var newBuilderItem = new ListItem(new ScrcpyCommandBuilderPage(null, null, _settingsManager))
            {
                Title = "Create New Preset in Command Builder",
                Subtitle = "Configure options and save as a new favorite preset",
                Icon = Classes.Icon.Setting,
                Tags = [new Tag("Builder"), new Tag("New Preset")],
            };
            _items.Add(newBuilderItem);
        }
    }

    public override void LoadMore()
    {
        HasMoreItems = false;
    }
}