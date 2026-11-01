// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class GamepadMode : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.gamepad-uhid",
            Title = "UHID Physical Gamepad",
            Arguments = "--gamepad=uhid",
            Description = "Simulate physical HID gamepad on Android using UHID (-G)",
            Category = "Control",
            Icon = Icon.Gamepad,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/gamepad.md#uhid",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.GamepadMode),
            "Gamepad Mode (--gamepad)",
            [
                ("Default", ""),
                ("UHID", "uhid"),
                ("AOA", "aoa"),
                ("Disabled", "disabled"),
            ],
            state.GamepadMode);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.GamepadMode = FormElementHelper.GetString(formInput, nameof(state.GamepadMode));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.GamepadMode))
        {
            args.Add($"--gamepad={state.GamepadMode.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.GamepadMode = string.Empty;
    }
}