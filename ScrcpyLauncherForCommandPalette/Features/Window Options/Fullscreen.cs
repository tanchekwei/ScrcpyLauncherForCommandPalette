// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class Fullscreen : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.fullscreen",
            Title = "Fullscreen",
            Arguments = "--fullscreen",
            Description = "Start mirroring directly in fullscreen mode (-f)",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#fullscreen",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.Fullscreen),
            "Fullscreen Window (-f, --fullscreen)",
            state.Fullscreen);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.Fullscreen = FormElementHelper.GetBool(formInput, nameof(state.Fullscreen));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.Fullscreen)
        {
            args.Add("--fullscreen");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.Fullscreen = false;
    }
}