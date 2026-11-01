// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;
using ScrcpyLauncherForCommandPalette.Pages;

namespace ScrcpyLauncherForCommandPalette.Services;

public sealed class FavoriteCommandsService
{
    private static readonly Lazy<FavoriteCommandsService> _lazy = new(() => new FavoriteCommandsService());
    public static FavoriteCommandsService Instance => _lazy.Value;

    private readonly List<(ScrcpyFavoriteCommand Favorite, ListItem Item)> _cache = [];
    private readonly object _lock = new();

    public IReadOnlyList<ListItem> GetCachedItems(string query = "")
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return [.. _cache.Select(x => x.Item)];
            }

            return [.. _cache
                .Where(x =>
                    x.Favorite.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    x.Favorite.Arguments.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    x.Favorite.FullCommand.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (x.Favorite.TargetDeviceModel != null && x.Favorite.TargetDeviceModel.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.Item)];
        }
    }

    public ListItem? GetCommandItem(string id)
    {
        lock (_lock)
        {
            return _cache.FirstOrDefault(x => x.Item.Command?.Id == id || x.Favorite.Id == id).Item;
        }
    }

    public void RebuildCache(IEnumerable<ScrcpyFavoriteCommand> favorites, SettingsManager settingsManager)
    {
        lock (_lock)
        {
            _cache.Clear();
            foreach (var fav in favorites)
            {
                var item = BuildFavoriteListItem(fav, settingsManager);
                _cache.Add((fav, item));
            }
        }
    }

    public static ListItem BuildFavoriteListItem(ScrcpyFavoriteCommand fav, SettingsManager settingsManager)
    {
        var runCmd = new RunScrcpyCommand($"Run {fav.Name}", fav.Arguments, Icon.Phone);
        // Command Builder Edit item for this favorite
        var editPage = new ScrcpyCommandBuilderPage(null, fav, settingsManager);
        var moreCommands = new List<ICommandContextItem>
        {
            new CommandContextItem(editPage),
            new CommandContextItem(new DuplicateFavoriteCommand(fav, settingsManager)),
            new CommandContextItem(new RunInConsoleScrcpyCommand(fav.Arguments)),
            new CommandContextItem(new RunDirectlyScrcpyCommand(fav.Arguments)),
            new CommandContextItem(new AddToAutoAdbFromFavoriteCommand(fav)),
            new CommandContextItem(new CopyScrcpyCommand(fav.FullCommand)),
            new CommandContextItem(new DeleteFavoriteCommand(fav.Id, settingsManager, fav.Name)),
        };

        var tags = new List<ITag> { new Tag("Favorite") };
        if (!string.IsNullOrWhiteSpace(fav.TargetDeviceModel))
        {
            tags.Add(new Tag(fav.TargetDeviceModel));
        }

        return new ListItem(runCmd)
        {
            Title = fav.Name,
            Subtitle = fav.FullCommand,
            Icon = Icon.Extension,
            Tags = [.. tags],
            MoreCommands = [.. moreCommands],
            Details = new Details() { Title = "Saved At", Body = $"{fav.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm}" }
        };
    }
}