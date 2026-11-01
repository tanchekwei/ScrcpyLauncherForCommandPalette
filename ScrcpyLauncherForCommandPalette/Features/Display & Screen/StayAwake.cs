// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class StayAwake : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.stay-awake",
            Title = "Stay Awake",
            Arguments = "--stay-awake",
            Description = "Prevent device from sleeping when plugged in (-w)",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#stay-awake",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.StayAwake),
            "Stay Awake while connected (-w, --stay-awake)",
            state.StayAwake);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.StayAwake = FormElementHelper.GetBool(formInput, nameof(state.StayAwake));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.StayAwake)
        {
            args.Add("--stay-awake");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.StayAwake = false;
    }
}