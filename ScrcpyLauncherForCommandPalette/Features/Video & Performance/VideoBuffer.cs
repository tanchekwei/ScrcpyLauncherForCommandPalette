// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoBuffer : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.video-buffer-50",
            Title = "Video Buffering (50ms)",
            Arguments = "--video-buffer=50",
            Description = "Add 50ms buffering for smoother video playback against network jitter",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#buffering",
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
            ("20ms (Low Latency)", "20"),
            ("50ms (Recommended for Wi-Fi)", "50"),
            ("100ms (Jitter Reduction)", "100"),
            ("200ms (High Jitter / Stable)", "200"),
            ("500ms (High Latency / Smooth)", "500"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.VideoBuffer, StringComparison.OrdinalIgnoreCase))
            ? (state.VideoBuffer ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedVideoBuffer",
            "Select Video Buffer (--video-buffer=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.VideoBuffer),
            "Or Custom Buffer in ms (--video-buffer=)",
            state.VideoBuffer,
            "e.g. 50 (milliseconds)"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedBuffer = FormElementHelper.GetString(formInput, "SelectedVideoBuffer");
        string manualBuffer = FormElementHelper.GetString(formInput, nameof(state.VideoBuffer));

        if (!string.IsNullOrWhiteSpace(manualBuffer) &&
            !string.Equals(manualBuffer, selectedBuffer, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualBuffer, state.VideoBuffer, StringComparison.OrdinalIgnoreCase))
        {
            state.VideoBuffer = manualBuffer;
        }
        else if (!string.IsNullOrWhiteSpace(selectedBuffer))
        {
            state.VideoBuffer = selectedBuffer;
        }
        else
        {
            state.VideoBuffer = manualBuffer;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoBuffer))
        {
            args.Add($"--video-buffer={state.VideoBuffer.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoBuffer = string.Empty;
    }
}