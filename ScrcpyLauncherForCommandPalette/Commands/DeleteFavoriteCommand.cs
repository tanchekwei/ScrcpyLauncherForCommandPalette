// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ScrcpyLauncherForCommandPalette.Commands;

public sealed partial class DeleteFavoriteCommand : InvokableCommand
{
    private readonly string _id;
    private readonly SettingsManager _settingsManager;
    private readonly string _name;

    public DeleteFavoriteCommand(string id, SettingsManager settingsManager, string name)
    {
        _id = id;
        _settingsManager = settingsManager;
        _name = name;
        Name = "Delete from Favorites";
        Icon = Classes.Icon.EraseTool;
    }

    public override CommandResult Invoke()
    {
        _settingsManager.RemoveFavorite(_id);
        new ToastStatusMessage($"Removed '{_name}' from favorites.").Show();
        return CommandResult.KeepOpen();
    }
}