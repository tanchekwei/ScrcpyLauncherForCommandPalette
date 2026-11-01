// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioCodec : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-aac",
            Title = "Audio Codec: AAC",
            Arguments = "--audio-codec=aac",
            Description = "Use AAC audio codec (helpful if device Opus encoder fails)",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#codec",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-flac",
            Title = "Audio Codec: FLAC",
            Arguments = "--audio-codec=flac",
            Description = "Use lossless FLAC audio codec",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#codec",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-raw",
            Title = "Audio Codec: RAW PCM",
            Arguments = "--audio-codec=raw",
            Description = "Use uncompressed 16-bit LE PCM raw audio stream",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#codec",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.AudioCodec),
            "Audio Codec (--audio-codec)",
            [
                ("Default (Opus)", ""),
                ("Opus", "opus"),
                ("AAC", "aac"),
                ("FLAC (Lossless)", "flac"),
                ("Raw PCM", "raw"),
            ],
            state.AudioCodec);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AudioCodec = FormElementHelper.GetString(formInput, nameof(state.AudioCodec));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioCodec))
        {
            args.Add($"--audio-codec={state.AudioCodec.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioCodec = string.Empty;
    }
}