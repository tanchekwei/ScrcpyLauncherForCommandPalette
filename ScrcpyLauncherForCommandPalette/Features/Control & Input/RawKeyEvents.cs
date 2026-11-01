// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class RawKeyEvents : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.raw-key-events",
            Title = "Raw Key Events (--raw-key-events)",
            Arguments = "--raw-key-events",
            Description = "Inject raw key events for all keys and ignore text events",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.RawKeyEvents),
            "Inject Raw Key Events (--raw-key-events)",
            state.RawKeyEvents);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RawKeyEvents = FormElementHelper.GetBool(formInput, nameof(state.RawKeyEvents));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.RawKeyEvents)
        {
            args.Add("--raw-key-events");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RawKeyEvents = false;
    }
}