// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class AlwaysOnTop : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.always-on-top",
            Title = "Always On Top",
            Arguments = "--always-on-top",
            Description = "Keep scrcpy window on top of all other windows",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#always-on-top",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.AlwaysOnTop),
            "Always On Top Window (--always-on-top)",
            state.AlwaysOnTop);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AlwaysOnTop = FormElementHelper.GetBool(formInput, nameof(state.AlwaysOnTop));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.AlwaysOnTop)
        {
            args.Add("--always-on-top");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AlwaysOnTop = false;
    }
}