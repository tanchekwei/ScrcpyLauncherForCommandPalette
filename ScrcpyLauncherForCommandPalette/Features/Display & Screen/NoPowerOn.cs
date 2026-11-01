// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class NoPowerOn : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-power-on",
            Title = "Do Not Power On At Start",
            Arguments = "--no-power-on",
            Description = "Do not wake up or power on the device screen on start",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#power-on-on-start",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoPowerOn),
            "Do Not Power On At Start (--no-power-on)",
            state.NoPowerOn);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoPowerOn = FormElementHelper.GetBool(formInput, nameof(state.NoPowerOn));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoPowerOn)
        {
            args.Add("--no-power-on");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoPowerOn = false;
    }
}