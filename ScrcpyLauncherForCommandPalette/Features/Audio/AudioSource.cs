// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioSource : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-mic",
            Title = "Capture Microphone Audio",
            Arguments = "--audio-source=mic",
            Description = "Capture device microphone instead of device audio output",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#source",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.AudioSource),
            "Audio Source (--audio-source)",
            [
                ("Default (Output)", ""),
                ("Output Playback", "playback"),
                ("Device Microphone", "mic"),
                ("Microphone (Raw/Unprocessed)", "mic-unprocessed"),
                ("Microphone (Camcorder)", "mic-camcorder"),
                ("Voice Recognition", "mic-voice-recognition"),
                ("Voice Communication", "mic-voice-communication"),
            ],
            state.AudioSource);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AudioSource = FormElementHelper.GetString(formInput, nameof(state.AudioSource));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioSource))
        {
            args.Add($"--audio-source={state.AudioSource.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioSource = string.Empty;
    }
}