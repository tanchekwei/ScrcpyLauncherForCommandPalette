// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class MouseMode : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.mouse-uhid",
            Title = "UHID Physical Mouse",
            Arguments = "--mouse=uhid",
            Description = "Simulate physical HID mouse with pointer capture (-M)",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/mouse.md#uhid",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.mouse-aoa",
            Title = "AOA Physical Mouse (USB)",
            Arguments = "--mouse=aoa",
            Description = "Simulate physical HID mouse via AOAv2 protocol directly over USB",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/mouse.md#aoa",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-mouse-hover",
            Title = "Disable Mouse Hover",
            Arguments = "--no-mouse-hover",
            Description = "Do not forward mouse hover movements without clicks",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/mouse.md#mouse-hover",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.MouseMode),
            "Mouse Mode (--mouse)",
            [
                ("Default", ""),
                ("SDK", "sdk"),
                ("UHID", "uhid"),
                ("AOA", "aoa"),
                ("Disabled", "disabled"),
            ],
            state.MouseMode);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.MouseMode = FormElementHelper.GetString(formInput, nameof(state.MouseMode));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.MouseMode))
        {
            args.Add($"--mouse={state.MouseMode.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.MouseMode = string.Empty;
    }
}