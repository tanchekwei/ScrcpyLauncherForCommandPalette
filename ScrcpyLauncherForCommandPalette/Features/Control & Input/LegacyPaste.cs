// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class LegacyPaste : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.legacy-paste",
            Title = "Legacy Paste (--legacy-paste)",
            Arguments = "--legacy-paste",
            Description = "Paste by simulating key events instead of setting device clipboard",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/control.md#copy-paste",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.LegacyPaste),
            "Simulate Keypresses on Paste (--legacy-paste)",
            state.LegacyPaste);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.LegacyPaste = FormElementHelper.GetBool(formInput, nameof(state.LegacyPaste));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.LegacyPaste)
        {
            args.Add("--legacy-paste");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.LegacyPaste = false;
    }
}