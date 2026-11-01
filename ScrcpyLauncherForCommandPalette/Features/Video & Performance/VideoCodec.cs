// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoCodec : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.video-h265",
            Title = "Video Codec: H.265 / HEVC",
            Arguments = "--video-codec=h265",
            Description = "Use H.265 (HEVC) video codec for superior visual quality",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#codec",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.video-av1",
            Title = "Video Codec: AV1",
            Arguments = "--video-codec=av1",
            Description = "Use AV1 video codec for supported devices",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#codec",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.VideoCodec),
            "Video Codec (--video-codec)",
            [
                ("Default (H.264)", ""),
                ("H.264", "h264"),
                ("H.265 / HEVC", "h265"),
                ("AV1", "av1"),
            ],
            state.VideoCodec);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.VideoCodec = FormElementHelper.GetString(formInput, nameof(state.VideoCodec));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoCodec))
        {
            args.Add($"--video-codec={state.VideoCodec.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoCodec = string.Empty;
    }
}