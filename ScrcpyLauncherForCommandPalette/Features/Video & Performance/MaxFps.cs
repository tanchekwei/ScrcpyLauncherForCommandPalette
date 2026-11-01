// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class MaxFps : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.fps-30",
            Title = "Limit Frame Rate: 30 FPS",
            Arguments = "--max-fps=30",
            Description = "Cap video capture frame rate at 30 FPS to save device battery",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#frame-rate",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.fps-60",
            Title = "Limit Frame Rate: 60 FPS",
            Arguments = "--max-fps=60",
            Description = "Cap video capture frame rate at 60 FPS for smooth playback",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#frame-rate",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.fps-120",
            Title = "Limit Frame Rate: 120 FPS",
            Arguments = "--max-fps=120",
            Description = "High refresh rate mirror for 120Hz displays",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#frame-rate",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.MaxFps),
            "Max Frame Rate (--max-fps)",
            [
                ("Default", ""),
                ("30 FPS", "30"),
                ("60 FPS", "60"),
                ("90 FPS", "90"),
                ("120 FPS", "120"),
            ],
            state.MaxFps);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.MaxFps = FormElementHelper.GetString(formInput, nameof(state.MaxFps));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.MaxFps))
        {
            args.Add($"--max-fps={state.MaxFps.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.MaxFps = string.Empty;
    }
}