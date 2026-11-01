// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class KeyboardMode : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.keyboard-uhid",
            Title = "UHID Physical Keyboard",
            Arguments = "--keyboard=uhid",
            Description = "Simulate physical HID keyboard via kernel module (-K) (supports IME & all chars)",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#uhid",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.keyboard-aoa",
            Title = "AOA Physical Keyboard (USB)",
            Arguments = "--keyboard=aoa",
            Description = "Simulate physical HID keyboard via AOAv2 protocol directly over USB",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#aoa",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.prefer-text",
            Title = "Prefer Text Injection",
            Arguments = "--prefer-text",
            Description = "Inject letters as text events instead of key events",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#text-injection-preference",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.raw-key-events",
            Title = "Raw Key Events (Games)",
            Arguments = "--raw-key-events",
            Description = "Force raw key events for WASD game controls",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#text-injection-preference",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-key-repeat",
            Title = "Disable Key Repeat",
            Arguments = "--no-key-repeat",
            Description = "Do not forward repeated key events when holding keys down",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#key-repeat",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.legacy-paste",
            Title = "Legacy Paste Mode",
            Arguments = "--legacy-paste",
            Description = "Inject clipboard text as key sequence instead of programmatic clipboard sync",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/control.md#copy-paste",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.KeyboardMode),
            "Keyboard Mode (--keyboard)",
            [
                ("Default", ""),
                ("SDK", "sdk"),
                ("UHID", "uhid"),
                ("AOA", "aoa"),
                ("Disabled", "disabled"),
            ],
            state.KeyboardMode);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.KeyboardMode = FormElementHelper.GetString(formInput, nameof(state.KeyboardMode));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.KeyboardMode))
        {
            args.Add($"--keyboard={state.KeyboardMode.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.KeyboardMode = string.Empty;
    }
}