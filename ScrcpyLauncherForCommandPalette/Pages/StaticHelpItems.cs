// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;

namespace ScrcpyLauncherForCommandPalette.Pages
{
    public static class StaticHelpItems
    {
        public static readonly ListItem OpenSettingsFolder = new(
            new OpenInExplorerCommand(Constant.SettingsFolderPath, "Open Extension Settings / Logs Folder")
        );

        public static ListItem SettingsItem { get; private set; } = null!;
        public static void Initialize(SettingsManager settingsManager)
        {
            try
            {
                SettingsItem = new ListItem(settingsManager.Settings.SettingsPage)
                {
                    Title = "Extension Setting",
                    Icon = Icon.Setting
                };
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
            }
        }
    }
}