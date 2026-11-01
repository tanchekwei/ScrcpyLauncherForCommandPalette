// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Commands
{
    internal sealed partial class OpenUrlCommand : InvokableCommand
    {
        private readonly string Url;

        public OpenUrlCommand(string url, string name, IconInfo icon)
        {
            try
            {
                Url = url;
                Name = name;
                Icon = icon;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }

        public override ICommandResult Invoke()
        {
            try
            {
                Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
                return CommandResult.Hide();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                return CommandResult.KeepOpen();
            }
        }
    }
}