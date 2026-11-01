// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class NoVideoPlayback : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoVideoPlayback),
            "Disable Video Playback (--no-video-playback)",
            state.NoVideoPlayback);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoVideoPlayback = FormElementHelper.GetBool(formInput, nameof(state.NoVideoPlayback));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoVideoPlayback)
        {
            args.Add("--no-video-playback");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoVideoPlayback = false;
    }
}