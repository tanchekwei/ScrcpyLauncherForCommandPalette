// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Commands;

public sealed partial class DuplicateFavoriteCommand : InvokableCommand
{
    private readonly ScrcpyFavoriteCommand _source;
    private readonly SettingsManager _settingsManager;

    public DuplicateFavoriteCommand(ScrcpyFavoriteCommand source, SettingsManager settingsManager)
    {
        _source = source;
        _settingsManager = settingsManager;

        Name = "Duplicate / Clone Preset";
        Icon = new IconInfo("\uE8C8");
        // Id = $"scrcpy.duplicate.favorite.{source.Id}";
    }

    public override CommandResult Invoke()
    {
        try
        {
            string baseName = _source.Name.Trim();
            string copyName = baseName.EndsWith("(Copy)", StringComparison.OrdinalIgnoreCase)
                ? $"{baseName[..^6].Trim()} (Copy 2)"
                : $"{baseName} (Copy)";

            var clone = new ScrcpyFavoriteCommand
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = copyName,
                Arguments = _source.Arguments,
                Description = _source.Description,
                TargetDeviceSerial = _source.TargetDeviceSerial,
                TargetDeviceModel = _source.TargetDeviceModel,
                CreatedAt = DateTime.UtcNow,
            };

            _settingsManager.AddFavorite(clone);
            new ToastStatusMessage($"Duplicated '{_source.Name}' as '{copyName}'").Show();
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            new ToastStatusMessage($"Failed to duplicate preset: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }
}