// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class CaptureOrientation : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.capture-orientation-90",
            Title = "Rotate Capture 90° Clockwise",
            Arguments = "--capture-orientation=90",
            Description = "Rotate mirrored video capture 90° clockwise",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.capture-orientation-lock",
            Title = "Lock Capture Orientation",
            Arguments = "--capture-orientation=@",
            Description = "Lock capture orientation to the initial angle regardless of device rotation",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#orientation",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.CaptureOrientation),
            "Capture Orientation (--capture-orientation)",
            [
                ("Default", ""),
                ("0° (Natural)", "0"),
                ("90° (Counterclockwise)", "90"),
                ("180° (Inverted)", "180"),
                ("270° (Clockwise)", "270"),
                ("Lock to Current Orientation (@)", "@"),
                ("Lock 0° (@0)", "@0"),
                ("Lock 90° (@90)", "@90"),
                ("Lock 180° (@180)", "@180"),
                ("Lock 270° (@270)", "@270"),
                ("Flip 0° (flip0)", "flip0"),
                ("Flip 90° (flip90)", "flip90"),
            ],
            state.CaptureOrientation);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CaptureOrientation = FormElementHelper.GetString(formInput, nameof(state.CaptureOrientation));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.CaptureOrientation))
        {
            args.Add($"--capture-orientation={state.CaptureOrientation.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CaptureOrientation = string.Empty;
    }
}