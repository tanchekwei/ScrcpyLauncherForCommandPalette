// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraFps : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-60fps",
            Title = "Camera 60 FPS",
            Arguments = "--video-source=camera --camera-fps=60",
            Description = "Capture device camera at 60 frames per second",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#frame-rate",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.CameraFps),
            "Camera FPS (--camera-fps)",
            [
                ("Default", ""),
                ("30 FPS", "30"),
                ("60 FPS", "60"),
            ],
            state.CameraFps);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraFps = FormElementHelper.GetString(formInput, nameof(state.CameraFps));
        if (!string.IsNullOrWhiteSpace(state.CameraFps))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraFps))
        {
            args.Add($"--camera-fps={state.CameraFps.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraFps = string.Empty;
    }
}