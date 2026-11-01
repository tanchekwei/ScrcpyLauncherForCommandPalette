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

public sealed class ListEncoders : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.list-encoders",
            Title = "List Encoders",
            Arguments = "--list-encoders",
            Description = "List available video and audio encoders on the device",
            Category = "Diagnostic",
            Icon = Icon.Terminal,
            RequiresConsole = true,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#encoder",
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        var serial = service.State?.DeviceSerial;
        var args = string.IsNullOrWhiteSpace(serial) ? "--list-encoders" : $"-s {serial} --list-encoders";
        return new ScrcpyCommandOutputPage("List Encoders", args, Icon.Terminal);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }
}