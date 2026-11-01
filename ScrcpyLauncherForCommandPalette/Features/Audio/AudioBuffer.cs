// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioBuffer : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-buffer-200",
            Title = "Audio Buffering (200ms)",
            Arguments = "--audio-buffer=200",
            Description = "Buffer audio 200ms to reduce glitching and buffer underrun",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#buffering",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.AudioBuffer),
            "Audio Buffer (--audio-buffer ms)",
            [
                ("Default (50ms)", ""),
                ("100ms", "100"),
                ("200ms (Reduce Glitches)", "200"),
                ("500ms (High Latency)", "500"),
            ],
            state.AudioBuffer);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AudioBuffer = FormElementHelper.GetString(formInput, nameof(state.AudioBuffer));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioBuffer))
        {
            args.Add($"--audio-buffer={state.AudioBuffer.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioBuffer = string.Empty;
    }
}