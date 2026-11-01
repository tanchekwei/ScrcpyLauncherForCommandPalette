// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class SelectUsb : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.select-usb",
            Title = "Select USB Device",
            Arguments = "--select-usb",
            Description = "Explicitly use the device connected over USB (-d)",
            Category = "Connection",
            Icon = Icon.Cast,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/connection.md#selection",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.SelectUsb),
            "Select USB Device (-d, --select-usb)",
            state.SelectUsb);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.SelectUsb = FormElementHelper.GetBool(formInput, nameof(state.SelectUsb));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.SelectUsb)
        {
            args.Add("-d");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.SelectUsb = false;
    }
}