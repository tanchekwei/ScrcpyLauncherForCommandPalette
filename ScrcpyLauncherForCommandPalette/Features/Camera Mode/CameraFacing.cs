// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraFacing : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-front",
            Title = "Mirror Front Camera",
            Arguments = "--video-source=camera --camera-facing=front",
            Description = "Capture device front-facing camera with microphone audio",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#selection",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-back",
            Title = "Mirror Back Camera",
            Arguments = "--video-source=camera --camera-facing=back",
            Description = "Capture device back-facing camera with microphone audio",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#selection",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.CameraFacing),
            "Camera Facing (--camera-facing)",
            [
                ("Default", ""),
                ("Back Camera", "back"),
                ("Front / Selfie Camera", "front"),
                ("External Camera", "external"),
            ],
            state.CameraFacing);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraFacing = FormElementHelper.GetString(formInput, nameof(state.CameraFacing));
        if (!string.IsNullOrWhiteSpace(state.CameraFacing))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraFacing))
        {
            args.Add($"--camera-facing={state.CameraFacing.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraFacing = string.Empty;
    }
}