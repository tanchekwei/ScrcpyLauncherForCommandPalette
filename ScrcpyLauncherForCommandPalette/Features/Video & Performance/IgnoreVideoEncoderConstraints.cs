// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class IgnoreVideoEncoderConstraints : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.ignore-video-encoder-constraints",
            Title = "Ignore Video Encoder Constraints",
            Arguments = "--ignore-video-encoder-constraints",
            Description = "Do not consider video encoder capabilities / constraints",
            Category = "Video",
            Icon = Icon.Video,
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.IgnoreVideoEncoderConstraints),
            "Ignore Video Encoder Constraints (--ignore-video-encoder-constraints)",
            state.IgnoreVideoEncoderConstraints);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.IgnoreVideoEncoderConstraints = FormElementHelper.GetBool(formInput, nameof(state.IgnoreVideoEncoderConstraints));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.IgnoreVideoEncoderConstraints)
        {
            args.Add("--ignore-video-encoder-constraints");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.IgnoreVideoEncoderConstraints = false;
    }
}