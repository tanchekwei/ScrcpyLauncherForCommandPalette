// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraSize : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-1080p",
            Title = "Camera 1080p (1920x1080)",
            Arguments = "--video-source=camera --camera-size=1920x1080",
            Description = "Capture device camera in Full HD 1080p resolution",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#size-selection",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.CameraSize),
            "Camera Resolution (--camera-size)",
            [
                ("Default", ""),
                ("1280x720 (720p)", "1280x720"),
                ("1920x1080 (1080p FHD)", "1920x1080"),
                ("3840x2160 (4K UHD)", "3840x2160"),
            ],
            state.CameraSize);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraSize = FormElementHelper.GetString(formInput, nameof(state.CameraSize));
        if (!string.IsNullOrWhiteSpace(state.CameraSize))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraSize))
        {
            args.Add($"--camera-size={state.CameraSize.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraSize = string.Empty;
    }
}