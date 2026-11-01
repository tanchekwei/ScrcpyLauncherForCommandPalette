// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Recording;

public sealed class RecordOrientation : IScrcpyFeature
{
    public string Category => "Recording";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.record-orientation",
            Title = "Record Orientation (--record-orientation)",
            Arguments = "--record-orientation=0",
            Description = "Set rotation for recorded file (0, 90, 180, 270) independently of display",
            Category = "Recording",
            Icon = Icon.Record,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("Default (matching display)", ""),
            ("0° (Normal)", "0"),
            ("90° Clockwise", "90"),
            ("180° Inverted", "180"),
            ("270° Counter-Clockwise", "270"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.RecordOrientation),
            "Record Orientation (--record-orientation)",
            choices,
            state.RecordOrientation);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RecordOrientation = FormElementHelper.GetString(formInput, nameof(state.RecordOrientation));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RecordOrientation))
        {
            args.Add($"--record-orientation={state.RecordOrientation}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RecordOrientation = string.Empty;
    }
}