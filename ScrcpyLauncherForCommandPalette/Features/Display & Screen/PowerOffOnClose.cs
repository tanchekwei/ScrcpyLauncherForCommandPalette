// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class PowerOffOnClose : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.power-off-on-close",
            Title = "Power Off On Close",
            Arguments = "--power-off-on-close",
            Description = "Turn device screen off automatically when scrcpy closes",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#power-off-on-close",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.PowerOffOnClose),
            "Power Off Screen on exit (--power-off-on-close)",
            state.PowerOffOnClose);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.PowerOffOnClose = FormElementHelper.GetBool(formInput, nameof(state.PowerOffOnClose));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.PowerOffOnClose)
        {
            args.Add("--power-off-on-close");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PowerOffOnClose = false;
    }
}