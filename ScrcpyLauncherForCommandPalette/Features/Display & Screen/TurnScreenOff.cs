// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class TurnScreenOff : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.turn-screen-off",
            Title = "Turn Screen Off",
            Arguments = "--turn-screen-off",
            Description = "Turn the physical device screen off while mirroring (-S)",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#turn-screen-off",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.TurnScreenOff),
            "Turn Physical Screen Off (-S, --turn-screen-off)",
            state.TurnScreenOff);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.TurnScreenOff = FormElementHelper.GetBool(formInput, nameof(state.TurnScreenOff));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.TurnScreenOff)
        {
            args.Add("--turn-screen-off");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.TurnScreenOff = false;
    }
}