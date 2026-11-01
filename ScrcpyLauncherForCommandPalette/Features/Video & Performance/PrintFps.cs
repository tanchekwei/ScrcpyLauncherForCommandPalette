// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class PrintFps : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.print-fps",
            Title = "Print FPS Counter",
            Arguments = "--print-fps",
            Description = "Print real-time video capture FPS to console",
            Category = "Video",
            Icon = Icon.Video,
            RequiresConsole = true,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#frame-rate",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.PrintFps),
            "Print FPS Counter (--print-fps)",
            state.PrintFps);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.PrintFps = FormElementHelper.GetBool(formInput, nameof(state.PrintFps));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.PrintFps)
        {
            args.Add("--print-fps");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PrintFps = false;
    }
}