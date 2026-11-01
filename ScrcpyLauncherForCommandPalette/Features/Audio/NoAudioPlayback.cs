// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class NoAudioPlayback : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-audio-playback",
            Title = "Disable Audio Playback (--no-audio-playback)",
            Arguments = "--no-audio-playback",
            Description = "Capture and record audio without playing it locally on the computer",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoAudioPlayback),
            "Disable Audio Playback (--no-audio-playback)",
            state.NoAudioPlayback);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoAudioPlayback = FormElementHelper.GetBool(formInput, nameof(state.NoAudioPlayback));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoAudioPlayback)
        {
            args.Add("--no-audio-playback");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoAudioPlayback = false;
    }
}