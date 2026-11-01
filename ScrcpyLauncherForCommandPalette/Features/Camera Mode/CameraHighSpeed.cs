// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraHighSpeed : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-high-speed",
            Title = "Camera High-Speed Capture",
            Arguments = "--video-source=camera --camera-high-speed",
            Description = "Enable Android camera high-speed capture mode for slow-motion recording",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#high-speed-capture",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.CameraHighSpeed),
            "Camera High-Speed Capture (--camera-high-speed)",
            state.CameraHighSpeed);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraHighSpeed = FormElementHelper.GetBool(formInput, nameof(state.CameraHighSpeed));
        if (state.CameraHighSpeed)
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && state.CameraHighSpeed)
        {
            args.Add("--camera-high-speed");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraHighSpeed = false;
    }
}