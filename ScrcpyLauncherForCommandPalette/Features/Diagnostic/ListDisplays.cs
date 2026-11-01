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

public sealed class ListDisplays : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.list-displays",
            Title = "List Displays",
            Arguments = "--list-displays",
            Description = "List available display IDs on the Android device",
            Category = "Diagnostic",
            Icon = Icon.Terminal,
            RequiresConsole = true,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#display",
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        var serial = service.State?.DeviceSerial;
        var args = string.IsNullOrWhiteSpace(serial) ? "--list-displays" : $"-s {serial} --list-displays";
        return new ScrcpyCommandOutputPage("List Displays", args, Icon.Terminal);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }
}