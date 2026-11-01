// Copyright (c) 2026 tanchekwei 
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Pages;
using ScrcpyLauncherForCommandPalette.Services;
using Windows.ApplicationModel;

namespace ScrcpyLauncherForCommandPalette;

public partial class ScrcpyLauncherForCommandPaletteCommandsProvider : CommandProvider
{
    private readonly SettingsManager _settingsManager;
    private readonly ScrcpyLauncherPage _page;

    public ScrcpyLauncherForCommandPaletteCommandsProvider(
        SettingsManager settingsManager,
        ScrcpyLauncherPage page)
    {
        try
        {
            Id = Package.Current.Id.Name;
            _settingsManager = settingsManager;
            DisplayName = Constant.ScrcpyLauncherDisplayName;
            Icon = Classes.Icon.Extension;
            Settings = _settingsManager.Settings;
            _page = page;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            _settingsManager = null!;
            _page = null!;
            throw;
        }
    }

    public override ICommandItem[] TopLevelCommands()
    {
        try
        {
            return [
                new CommandItem(_page)
                {
                    Title = Constant.ScrcpyLauncherDisplayName,
                    Subtitle = "Run and manage scrcpy commands and connected Android devices",
                    Icon = Classes.Icon.Extension,
                    MoreCommands = [
                        new CommandContextItem(Settings!.SettingsPage),
                    ],
                },
            ];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return [];
        }
    }

    public override IFallbackCommandItem[] FallbackCommands() => [];

    public override ICommandItem? GetCommandItem(string id)
    {
        return FavoriteCommandsService.Instance.GetCommandItem(id);
    }
}