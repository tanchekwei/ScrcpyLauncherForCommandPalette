// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Recording;

public sealed class RecordFormat : IScrcpyFeature
{
    public string Category => "Recording";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.RecordFormat),
            "Record Format (--record-format=)",
            [
                ("Auto (by file extension)", ""),
                ("MP4", "mp4"),
                ("MKV", "mkv"),
                ("M4A", "m4a"),
                ("OPUS", "opus"),
                ("FLAC", "flac"),
                ("WAV", "wav"),
            ],
            state.RecordFormat);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RecordFormat = FormElementHelper.GetString(formInput, nameof(state.RecordFormat));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RecordFile) && !string.IsNullOrWhiteSpace(state.RecordFormat))
        {
            args.Add($"--record-format={state.RecordFormat.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RecordFormat = string.Empty;
    }
}