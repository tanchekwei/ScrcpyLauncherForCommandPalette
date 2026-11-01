// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class PreferText : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.prefer-text",
            Title = "Prefer Text Injection (--prefer-text)",
            Arguments = "--prefer-text",
            Description = "Inject letters and spaces as text events (fixes international keyboards in SDK mode)",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/keyboard.md#text-injection-preference",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.PreferText),
            "Prefer Text Injection (--prefer-text)",
            state.PreferText);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.PreferText = FormElementHelper.GetBool(formInput, nameof(state.PreferText));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.PreferText)
        {
            args.Add("--prefer-text");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PreferText = false;
    }
}