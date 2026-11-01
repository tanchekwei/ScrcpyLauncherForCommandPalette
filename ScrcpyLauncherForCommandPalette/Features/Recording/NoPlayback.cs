// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Recording;

public sealed class NoPlayback : IScrcpyFeature
{
    public string Category => "Recording";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.record-no-playback",
            Title = "Record Without Display (Background)",
            Arguments = "--no-playback --record=scrcpy_record.mp4",
            Description = "Record screen and audio directly without opening playback window",
            Category = "Recording",
            Icon = Icon.Record,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/recording.md#no-playback",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoPlayback),
            "Disable Screen Playback while Recording (--no-playback)",
            state.NoPlayback);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoPlayback = FormElementHelper.GetBool(formInput, nameof(state.NoPlayback));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RecordFile) && state.NoPlayback)
        {
            args.Add("--no-playback");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoPlayback = false;
    }
}