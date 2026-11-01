// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Recording;

public sealed class RecordFile : IScrcpyFeature
{
    public string Category => "Recording";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.record-mp4",
            Title = "Record to MP4",
            Arguments = "--record=scrcpy_record.mp4",
            Description = "Record screen and audio to scrcpy_record.mp4 file while mirroring",
            Category = "Recording",
            Icon = Icon.Record,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/recording.md",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.record-mkv",
            Title = "Record to MKV",
            Arguments = "--record=scrcpy_record.mkv",
            Description = "Record screen and audio to scrcpy_record.mkv file while mirroring",
            Category = "Recording",
            Icon = Icon.Record,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/recording.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var container = new JsonObject
        {
            ["type"] = "Container",
            ["items"] = new JsonArray(),
        };
        var items = (JsonArray)container["items"]!;

        var choices = new List<(string Title, string Value)>
        {
            ("(None / Custom)", ""),
            ("recording.mp4", "recording.mp4"),
            ("recording.mkv", "recording.mkv"),
            ("scrcpy_record.mp4", "scrcpy_record.mp4"),
            ("scrcpy_record.mkv", "scrcpy_record.mkv"),
            ("screen_capture.mp4", "screen_capture.mp4"),
            ("screen_capture.mkv", "screen_capture.mkv"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.RecordFile, StringComparison.OrdinalIgnoreCase))
            ? (state.RecordFile ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedRecordFile",
            "Select Record File Preset (--record=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.RecordFile),
            "Or Custom File Path (--record=)",
            state.RecordFile,
            "e.g. recording.mp4"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedFile = FormElementHelper.GetString(formInput, "SelectedRecordFile");
        string manualFile = FormElementHelper.GetString(formInput, nameof(state.RecordFile));

        if (!string.IsNullOrWhiteSpace(manualFile) &&
            !string.Equals(manualFile, selectedFile, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualFile, state.RecordFile, StringComparison.OrdinalIgnoreCase))
        {
            state.RecordFile = manualFile;
        }
        else if (!string.IsNullOrWhiteSpace(selectedFile))
        {
            state.RecordFile = selectedFile;
        }
        else
        {
            state.RecordFile = manualFile;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RecordFile))
        {
            args.Add($"--record=\"{state.RecordFile.Trim()}\"");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RecordFile = string.Empty;
    }
}