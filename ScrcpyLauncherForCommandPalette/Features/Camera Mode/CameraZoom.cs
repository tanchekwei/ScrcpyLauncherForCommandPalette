// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraZoom : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-zoom",
            Title = "Camera Zoom (--camera-zoom)",
            Arguments = "--camera-zoom=1.5",
            Description = "Set initial camera zoom factor (e.g. 1.0, 1.5, 2.0)",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#zoom",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var container = new JsonObject
        {
            ["type"] = "Container",
            ["items"] = new JsonArray(),
        };
        var items = (JsonArray)container["items"]!;

        var choices = new List<(string Title, string Value)>
        {
            ("(Default / Custom)", ""),
            ("1.0x (Normal / No Zoom)", "1.0"),
            ("1.5x Zoom", "1.5"),
            ("2.0x Zoom", "2.0"),
            ("3.0x Zoom", "3.0"),
            ("5.0x Zoom", "5.0"),
            ("10.0x Zoom", "10.0"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.CameraZoom, StringComparison.OrdinalIgnoreCase))
            ? (state.CameraZoom ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedCameraZoom",
            "Select Camera Zoom Factor (--camera-zoom)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.CameraZoom),
            "Or Custom Zoom Factor (--camera-zoom)",
            state.CameraZoom,
            "e.g. 1.5, 2.0"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedZoom = FormElementHelper.GetString(formInput, "SelectedCameraZoom");
        string manualZoom = FormElementHelper.GetString(formInput, nameof(state.CameraZoom));

        if (!string.IsNullOrWhiteSpace(manualZoom) &&
            !string.Equals(manualZoom, selectedZoom, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualZoom, state.CameraZoom, StringComparison.OrdinalIgnoreCase))
        {
            state.CameraZoom = manualZoom;
        }
        else if (!string.IsNullOrWhiteSpace(selectedZoom))
        {
            state.CameraZoom = selectedZoom;
        }
        else
        {
            state.CameraZoom = manualZoom;
        }

        if (!string.IsNullOrWhiteSpace(state.CameraZoom))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraZoom))
        {
            args.Add($"--camera-zoom={state.CameraZoom}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraZoom = string.Empty;
    }
}