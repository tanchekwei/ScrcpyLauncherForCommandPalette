// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class MaxSize : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.max-size-1024",
            Title = "Limit Resolution (1024px)",
            Arguments = "--max-size=1024",
            Description = "Downscale screen to max 1024px for higher performance and lower bandwidth (-m 1024)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#size",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.max-size-1920",
            Title = "Limit Resolution (1920px)",
            Arguments = "--max-size=1920",
            Description = "Downscale screen to max 1920px (Full HD) (-m 1920)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#size",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.MaxSize),
            "Max Dimension / Resolution (--max-size / -m)",
            [
                ("Default (Original)", ""),
                ("480p", "480"),
                ("720p (HD)", "720"),
                ("1080p (Full HD)", "1080"),
                ("1440p (2K)", "1440"),
                ("1920", "1920"),
            ],
            state.MaxSize);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.MaxSize = FormElementHelper.GetString(formInput, nameof(state.MaxSize));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.MaxSize))
        {
            args.Add($"--max-size={state.MaxSize.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.MaxSize = string.Empty;
    }
}