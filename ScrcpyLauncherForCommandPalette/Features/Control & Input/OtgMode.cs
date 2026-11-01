// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class OtgMode : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.otg",
            Title = "OTG Mode (No ADB)",
            Arguments = "--otg",
            Description = "Control device via AOA keyboard & mouse without USB debugging (ADB)",
            Category = "OTG",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/otg.md",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.otg-gamepad",
            Title = "OTG Mode with Gamepad",
            Arguments = "--otg -G",
            Description = "OTG control including physical gamepad forwarding over USB",
            Category = "OTG",
            Icon = Icon.Gamepad,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/otg.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.OtgMode),
            "OTG Mode (Simulate Physical Keyboard & Mouse) (--otg)",
            state.OtgMode);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.OtgMode = FormElementHelper.GetBool(formInput, nameof(state.OtgMode));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.OtgMode)
        {
            args.Add("--otg");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.OtgMode = false;
    }
}