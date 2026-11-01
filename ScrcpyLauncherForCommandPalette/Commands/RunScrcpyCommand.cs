// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Diagnostics;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Commands;

public sealed partial class RunScrcpyCommand : InvokableCommand
{
    private readonly string _arguments;
    private readonly bool? _requiresConsole;

    public RunScrcpyCommand(string title, string arguments, IconInfo? icon = null, bool? requiresConsole = null, string? id = null)
    {
        _arguments = arguments;
        _requiresConsole = requiresConsole;
        Name = title;
        Icon = icon ?? Classes.Icon.Extension;
        Id = id ?? $"scrcpy.{arguments}".Replace(' ', '.').ToLowerInvariant();
    }

    public override CommandResult Invoke()
    {
        return ScrcpyRunner.Run(_arguments, inConsole: _requiresConsole);
    }
}

public sealed partial class RunInConsoleScrcpyCommand : InvokableCommand
{
    private readonly string _arguments;

    public RunInConsoleScrcpyCommand(string arguments)
    {
        _arguments = arguments;
        Name = "Run in Console";
        Icon = new IconInfo("\uE756");
    }

    public override CommandResult Invoke()
    {
        return ScrcpyRunner.Run(_arguments, inConsole: true);
    }
}

public sealed partial class RunDirectlyScrcpyCommand : InvokableCommand
{
    private readonly string _arguments;

    public RunDirectlyScrcpyCommand(string arguments)
    {
        _arguments = arguments;
        Name = "Run Directly (No Console)";
        Icon = new IconInfo("\uE768");
    }

    public override CommandResult Invoke()
    {
        return ScrcpyRunner.Run(_arguments, inConsole: false);
    }
}

public sealed partial class CopyScrcpyCommand : InvokableCommand
{
    private readonly string _commandText;

    public CopyScrcpyCommand(string commandText)
    {
        _commandText = commandText;
        Name = "Copy Command";
        Icon = new IconInfo("\uE8A7");
    }

    public override CommandResult Invoke()
    {
        ScrcpyRunner.CopyToClipboard(_commandText);
        return CommandResult.KeepOpen();
    }
}

public sealed partial class OpenUrlDocCommand : InvokableCommand
{
    private readonly string _url;

    public OpenUrlDocCommand(string title, string url)
    {
        _url = url;
        Name = title;
        Icon = Classes.Icon.Web;
    }

    public override CommandResult Invoke()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _url,
                UseShellExecute = true,
            });
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return CommandResult.KeepOpen();
        }
    }
}