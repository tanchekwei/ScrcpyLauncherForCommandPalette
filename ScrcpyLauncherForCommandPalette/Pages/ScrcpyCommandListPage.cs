// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Commands;
using ScrcpyLauncherForCommandPalette.Helpers;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyCommandListPage : ListPage
{
    private readonly DeviceInfo? _targetDevice;

    public ScrcpyCommandListPage(DeviceInfo? targetDevice = null)
    {
        _targetDevice = targetDevice;

        if (_targetDevice != null)
        {
            Name = $"{_targetDevice.Model} Commands";
            Title = $"scrcpy Commands ({_targetDevice.Model})";
            Icon = Classes.Icon.Phone;
            // Id = $"ScrcpyCommandListPage.{_targetDevice.Serial}";
        }
        else
        {
            Name = "All scrcpy Commands";
            Title = "All scrcpy Commands";
            Icon = Classes.Icon.Extension;
            // Id = "ScrcpyCommandListPage.All";
        }
    }

    public override IListItem[] GetItems()
    {
        try
        {
            var items = new List<IListItem>();
            foreach (var cmd in ScrcpyCommands.All)
            {
                string actualArgs;
                if (_targetDevice != null)
                {
                    actualArgs = string.IsNullOrEmpty(cmd.Arguments)
                        ? $"-s {_targetDevice.Serial}"
                        : $"-s {_targetDevice.Serial} {cmd.Arguments}";
                }
                else
                {
                    actualArgs = cmd.Arguments;
                }

                var fullCmd = string.IsNullOrEmpty(actualArgs) ? "scrcpy" : $"scrcpy {actualArgs}";
                var runCommand = new RunScrcpyCommand(cmd.Title, actualArgs, cmd.Icon)
                {
                    // Id = _targetDevice != null ? $"{cmd.Id}.{_targetDevice.Serial}" : cmd.Id
                };

                var moreCommands = new List<ICommandContextItem>
                {
                    new CommandContextItem(new RunInConsoleScrcpyCommand(actualArgs)),
                    new CommandContextItem(new RunDirectlyScrcpyCommand(actualArgs)),
                    new CommandContextItem(new CopyScrcpyCommand(fullCmd)),
                };

                if (!string.IsNullOrEmpty(cmd.DocumentationUrl))
                {
                    moreCommands.Add(new CommandContextItem(new OpenUrlDocCommand("Documentation", cmd.DocumentationUrl)));
                }

                var item = new ListItem(runCommand)
                {
                    Title = cmd.Title,
                    Subtitle = $"{fullCmd}  —  {cmd.Description}",
                    Icon = cmd.Icon,
                    Tags = _targetDevice != null
                        ? [new Tag(cmd.Category), new Tag(_targetDevice.Model)]
                        : [new Tag(cmd.Category)],
                    MoreCommands = [.. moreCommands],
                };

                items.Add(item);
            }

            return [.. items];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return [];
        }
    }
}