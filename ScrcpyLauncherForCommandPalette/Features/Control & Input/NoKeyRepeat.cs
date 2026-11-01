// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class NoKeyRepeat : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-key-repeat",
            Title = "Disable Key Repeat (--no-key-repeat)",
            Arguments = "--no-key-repeat",
            Description = "Do not forward repeated key events when a key is held down",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#key-repeat",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoKeyRepeat),
            "Disable Key Repeat (--no-key-repeat)",
            state.NoKeyRepeat);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoKeyRepeat = FormElementHelper.GetBool(formInput, nameof(state.NoKeyRepeat));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoKeyRepeat)
        {
            args.Add("--no-key-repeat");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoKeyRepeat = false;
    }
}