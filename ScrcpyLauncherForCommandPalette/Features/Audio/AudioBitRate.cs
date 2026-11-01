// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioBitRate : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.AudioBitRate),
            "Audio Bitrate (--audio-bit-rate)",
            [
                ("Default (128 Kbps)", ""),
                ("64 Kbps", "64K"),
                ("128 Kbps", "128K"),
                ("192 Kbps", "192K"),
                ("256 Kbps", "256K"),
            ],
            state.AudioBitRate);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AudioBitRate = FormElementHelper.GetString(formInput, nameof(state.AudioBitRate));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioBitRate))
        {
            args.Add($"--audio-bit-rate={state.AudioBitRate.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioBitRate = string.Empty;
    }
}