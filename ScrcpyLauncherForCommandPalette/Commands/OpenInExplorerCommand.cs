// Modifications copyright (c) 2025 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Commands
{
    public sealed partial class OpenInExplorerCommand : InvokableCommand
    {
        private readonly string _path;
        private string _arguments;

        public OpenInExplorerCommand(string arguments, string name = "Open in Explorer", string path = "explorer.exe")
        {
            try
            {
                Name = name;
                _path = path;
                _arguments = arguments;
                Icon = Classes.Icon.FileExplorer;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }

        public override CommandResult Invoke()
        {
            try
            {
                string pathToOpen = _arguments;
                if (string.IsNullOrEmpty(pathToOpen))
                {
                    new ToastStatusMessage($"Path does not exist").Show();
                    return CommandResult.KeepOpen();
                }

                var pathInvalidResult = CommandHelpers.IsPathValid(pathToOpen);
                if (pathInvalidResult != null)
                {
                    return pathInvalidResult;
                }

                ShellHelpers.OpenInShell(_path, pathToOpen);
                return CommandResult.Dismiss();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                return CommandResult.KeepOpen();
            }
        }
    }
}
