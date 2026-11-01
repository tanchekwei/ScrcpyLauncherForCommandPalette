// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class DisplayOrientation : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.display-orientation-0",
            Title = "Display Orientation 0°",
            Arguments = "--display-orientation=0",
            Description = "Set initial display orientation to normal (0°)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.display-orientation-90",
            Title = "Display Orientation 90°",
            Arguments = "--display-orientation=90",
            Description = "Set initial display orientation to 90° counterclockwise",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.display-orientation-180",
            Title = "Display Orientation 180°",
            Arguments = "--display-orientation=180",
            Description = "Set initial display orientation to 180° (upside down)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.display-orientation-270",
            Title = "Display Orientation 270°",
            Arguments = "--display-orientation=270",
            Description = "Set initial display orientation to 270° counterclockwise",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.DisplayOrientation),
            "Initial Display Orientation (--display-orientation=)",
            [
                ("Default (0°)", ""),
                ("0° (Normal)", "0"),
                ("90° (Counterclockwise)", "90"),
                ("180° (Upside down)", "180"),
                ("270° (Clockwise)", "270"),
                ("flip0 (Flipped horizontally)", "flip0"),
                ("flip90 (Flipped 90°)", "flip90"),
                ("flip180 (Flipped 180°)", "flip180"),
                ("flip270 (Flipped 270°)", "flip270"),
            ],
            state.DisplayOrientation);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.DisplayOrientation = FormElementHelper.GetString(formInput, nameof(state.DisplayOrientation));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.DisplayOrientation))
        {
            args.Add($"--display-orientation={state.DisplayOrientation.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.DisplayOrientation = string.Empty;
    }
}