// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoEncoder : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.video-encoder",
            Title = "Video Encoder (--video-encoder)",
            Arguments = "--video-encoder=name",
            Description = "Select a specific video MediaCodec encoder (from --list-encoders)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#encoder",
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
            ("(Default / Custom)", ""),
            ("c2.android.avc.encoder (Android Software H.264)", "c2.android.avc.encoder"),
            ("c2.android.hevc.encoder (Android Software H.265)", "c2.android.hevc.encoder"),
            ("OMX.google.h264.encoder (Google Software H.264)", "OMX.google.h264.encoder"),
            ("OMX.qcom.video.encoder.avc (Qualcomm Hardware H.264)", "OMX.qcom.video.encoder.avc"),
            ("c2.qti.avc.encoder (Qualcomm Hardware H.264 v2)", "c2.qti.avc.encoder"),
            ("OMX.Exynos.AVC.Encoder (Samsung Hardware H.264)", "OMX.Exynos.AVC.Encoder"),
            ("OMX.MTK.VIDEO.ENCODER.AVC (MediaTek Hardware H.264)", "OMX.MTK.VIDEO.ENCODER.AVC"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.VideoEncoder, StringComparison.OrdinalIgnoreCase))
            ? (state.VideoEncoder ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedVideoEncoder",
            "Select Video Encoder Preset (--video-encoder)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.VideoEncoder),
            "Or Custom Video Encoder (--video-encoder)",
            state.VideoEncoder,
            "e.g. c2.android.avc.encoder, OMX.qcom.video.encoder.avc"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedEncoder = FormElementHelper.GetString(formInput, "SelectedVideoEncoder");
        string manualEncoder = FormElementHelper.GetString(formInput, nameof(state.VideoEncoder));

        if (!string.IsNullOrWhiteSpace(manualEncoder) &&
            !string.Equals(manualEncoder, selectedEncoder, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualEncoder, state.VideoEncoder, StringComparison.OrdinalIgnoreCase))
        {
            state.VideoEncoder = manualEncoder;
        }
        else if (!string.IsNullOrWhiteSpace(selectedEncoder))
        {
            state.VideoEncoder = selectedEncoder;
        }
        else
        {
            state.VideoEncoder = manualEncoder;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoEncoder))
        {
            args.Add($"--video-encoder={state.VideoEncoder}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoEncoder = string.Empty;
    }
}