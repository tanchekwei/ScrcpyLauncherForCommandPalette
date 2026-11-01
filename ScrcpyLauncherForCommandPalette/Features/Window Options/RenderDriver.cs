// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class RenderDriver : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.render-driver",
            Title = "Render Driver (--render-driver)",
            Arguments = "--render-driver=direct3d",
            Description = "Request SDL to use a specific render driver",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("Default (auto)", ""),
            ("Direct3D", "direct3d"),
            ("OpenGL", "opengl"),
            ("OpenGL ES 2.0", "opengles2"),
            ("Software", "software"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.RenderDriver),
            "Render Driver (--render-driver)",
            choices,
            state.RenderDriver);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RenderDriver = FormElementHelper.GetString(formInput, nameof(state.RenderDriver));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RenderDriver))
        {
            args.Add($"--render-driver={state.RenderDriver}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RenderDriver = string.Empty;
    }
}