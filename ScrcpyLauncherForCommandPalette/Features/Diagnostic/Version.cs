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

public sealed class Version : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.version",
            Title = "Version",
            Arguments = "--version",
            Description = "Show scrcpy version and build dependencies",
            Category = "Diagnostic",
            Icon = Icon.Info,
            RequiresConsole = true,
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        return new ScrcpyCommandOutputPage("scrcpy Version", "--version", Icon.Info);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }
}