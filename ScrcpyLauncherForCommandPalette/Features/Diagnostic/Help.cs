// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Pages;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Features.Diagnostic;

public sealed class Help : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.help",
            Title = "Help",
            Arguments = "--help",
            Description = "Show full command-line reference in console",
            Category = "Diagnostic",
            Icon = Icon.Help,
            RequiresConsole = true,
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        return new ScrcpyCommandOutputPage("scrcpy Help", "--help", Icon.Help);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }
}