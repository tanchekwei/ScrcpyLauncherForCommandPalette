// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoBitRate : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.bit-rate-2m",
            Title = "Video Bit Rate: 2 Mbps",
            Arguments = "--video-bit-rate=2M",
            Description = "Reduce video bit rate to 2 Mbps for low latency / slow connections (-b 2M)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#bit-rate",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.bit-rate-4m",
            Title = "Video Bit Rate: 4 Mbps",
            Arguments = "--video-bit-rate=4M",
            Description = "Set video bit rate to 4 Mbps (-b 4M)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#bit-rate",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.bit-rate-16m",
            Title = "Video Bit Rate: 16 Mbps (High Quality)",
            Arguments = "--video-bit-rate=16M",
            Description = "High quality video bit rate at 16 Mbps (-b 16M)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#bit-rate",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.VideoBitRate),
            "Video Bitrate (--video-bit-rate / -b)",
            [
                ("Default (8 Mbps)", ""),
                ("2 Mbps", "2M"),
                ("4 Mbps", "4M"),
                ("8 Mbps", "8M"),
                ("16 Mbps", "16M"),
                ("24 Mbps", "24M"),
            ],
            state.VideoBitRate);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.VideoBitRate = FormElementHelper.GetString(formInput, nameof(state.VideoBitRate));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoBitRate))
        {
            args.Add($"--video-bit-rate={state.VideoBitRate.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoBitRate = string.Empty;
    }
}