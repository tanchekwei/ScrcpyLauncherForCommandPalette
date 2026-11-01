// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CameraMode;

public sealed class CameraId : IScrcpyFeature
{
    public string Category => "Camera Mode";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.camera-id",
            Title = "Camera ID (--camera-id)",
            Arguments = "--camera-id=0",
            Description = "Specify camera numerical ID (retrieved from --list-cameras)",
            Category = "Camera",
            Icon = Icon.Camera,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/camera.md#selection",
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
            ("0 (Default Back Camera)", "0"),
            ("1 (Default Front Camera)", "1"),
            ("2 (Secondary / Ultra-Wide)", "2"),
            ("3 (Telephoto / Macro)", "3"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.CameraId, StringComparison.OrdinalIgnoreCase))
            ? (state.CameraId ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedCameraId",
            "Select Camera ID (--camera-id)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.CameraId),
            "Or Custom Camera ID (--camera-id)",
            state.CameraId,
            "e.g. 0, 1, 2 (from --list-cameras)"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedId = FormElementHelper.GetString(formInput, "SelectedCameraId");
        string manualId = FormElementHelper.GetString(formInput, nameof(state.CameraId));

        if (!string.IsNullOrWhiteSpace(manualId) &&
            !string.Equals(manualId, selectedId, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualId, state.CameraId, StringComparison.OrdinalIgnoreCase))
        {
            state.CameraId = manualId;
        }
        else if (!string.IsNullOrWhiteSpace(selectedId))
        {
            state.CameraId = selectedId;
        }
        else
        {
            state.CameraId = manualId;
        }

        if (!string.IsNullOrWhiteSpace(state.CameraId))
        {
            state.CameraSource = true;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.CameraSource && !string.IsNullOrWhiteSpace(state.CameraId))
        {
            args.Add($"--camera-id={state.CameraId}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.CameraId = string.Empty;
    }
}