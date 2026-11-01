// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class DeviceSerial : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateTextInput(
            nameof(state.DeviceSerial),
            "Target Device Serial (-s)",
            state.DeviceSerial,
            "e.g. 192.168.1.100:5555 or device serial");
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.DeviceSerial = FormElementHelper.GetString(formInput, nameof(state.DeviceSerial));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.DeviceSerial))
        {
            args.Add($"-s {state.DeviceSerial.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.DeviceSerial = string.Empty;
    }
}