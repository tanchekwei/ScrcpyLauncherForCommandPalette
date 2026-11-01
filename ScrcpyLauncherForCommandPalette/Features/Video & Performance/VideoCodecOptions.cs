// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoCodecOptions : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.video-codec-options",
            Title = "Video Codec Options (--video-codec-options)",
            Arguments = "--video-codec-options=key:type=val",
            Description = "Pass low-level MediaFormat options to the device video encoder",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#codec-options",
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
            ("i-frame-interval=10 (Keyframe every 10s)", "i-frame-interval=10"),
            ("repeat-previous-frame-after=50000 (50ms Repeat for static screen)", "repeat-previous-frame-after=50000"),
            ("repeat-previous-frame-after=100000 (100ms Repeat for static screen)", "repeat-previous-frame-after=100000"),
            ("bitrate-mode=1 (VBR - Variable Bitrate)", "bitrate-mode=1"),
            ("bitrate-mode=2 (CBR - Constant Bitrate)", "bitrate-mode=2"),
            ("profile=1,level=512 (Baseline Profile)", "profile=1,level=512"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.VideoCodecOptions, StringComparison.OrdinalIgnoreCase))
            ? (state.VideoCodecOptions ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedVideoCodecOptions",
            "Select Video Codec Option Preset (--video-codec-options)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.VideoCodecOptions),
            "Or Custom Codec Options (--video-codec-options)",
            state.VideoCodecOptions,
            "e.g. i-frame-interval=10,repeat-previous-frame-after=50000"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedOpt = FormElementHelper.GetString(formInput, "SelectedVideoCodecOptions");
        string manualOpt = FormElementHelper.GetString(formInput, nameof(state.VideoCodecOptions));

        if (!string.IsNullOrWhiteSpace(manualOpt) &&
            !string.Equals(manualOpt, selectedOpt, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualOpt, state.VideoCodecOptions, StringComparison.OrdinalIgnoreCase))
        {
            state.VideoCodecOptions = manualOpt;
        }
        else if (!string.IsNullOrWhiteSpace(selectedOpt))
        {
            state.VideoCodecOptions = selectedOpt;
        }
        else
        {
            state.VideoCodecOptions = manualOpt;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoCodecOptions))
        {
            args.Add($"--video-codec-options={state.VideoCodecOptions}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoCodecOptions = string.Empty;
    }
}