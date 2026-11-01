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

public sealed class ListCameras : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Diagnostic";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.list-cameras",
            Title = "List Available Cameras",
            Arguments = "--list-cameras",
            Description = "List all cameras available on the device with IDs and directions",
            Category = "Camera",
            Icon = Icon.Camera,
            RequiresConsole = true,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#list",
        },
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        var serial = service.State?.DeviceSerial;
        var args = string.IsNullOrWhiteSpace(serial) ? "--list-cameras" : $"-s {serial} --list-cameras";
        return new ScrcpyCommandOutputPage("List Available Cameras", args, Icon.Camera);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state) => null;

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput) { }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args) { }

    public void ResetState(ScrcpyCommandBuilderState state) { }
}