// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraTorch : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-torch",
            Title = "Camera with Torch / Flashlight",
            Arguments = "--video-source=camera --camera-torch",
            Description = "Mirror camera with camera torch / flashlight turned on",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#torch",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.CameraTorch),
            "Camera Torch / Flashlight (--camera-torch)",
            state.CameraTorch);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraTorch = FormElementHelper.GetBool(formInput, nameof(state.CameraTorch));
        if (state.CameraTorch)
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && state.CameraTorch)
        {
            args.Add("--camera-torch");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraTorch = false;
    }
}