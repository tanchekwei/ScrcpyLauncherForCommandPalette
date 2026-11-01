// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraAr : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-ar",
            Title = "Camera Aspect Ratio (--camera-ar)",
            Arguments = "--camera-ar=16:9",
            Description = "Select camera size matching aspect ratio (e.g. 4:3, 16:9, sensor)",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#size-selection",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("None / Default", ""),
            ("16:9", "16:9"),
            ("4:3", "4:3"),
            ("1:1", "1:1"),
            ("Sensor Native", "sensor"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.CameraAr),
            "Camera Aspect Ratio (--camera-ar)",
            choices,
            state.CameraAr);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.CameraAr = FormElementHelper.GetString(formInput, nameof(state.CameraAr));
        if (!string.IsNullOrWhiteSpace(state.CameraAr))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraAr))
        {
            args.Add($"--camera-ar={state.CameraAr}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraAr = string.Empty;
    }
}