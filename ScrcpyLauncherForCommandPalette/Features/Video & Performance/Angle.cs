// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class Angle : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.angle",
            Title = "Video Rotation Angle (--angle)",
            Arguments = "--angle=90",
            Description = "Rotate video content by a custom angle in degrees clockwise",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#angle",
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
            ("(None / Custom)", ""),
            ("0° (Normal)", "0"),
            ("90° (Clockwise)", "90"),
            ("180° (Upside Down)", "180"),
            ("270° (Counter-Clockwise)", "270"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.Angle, StringComparison.OrdinalIgnoreCase))
            ? (state.Angle ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedAngle",
            "Select Rotation Angle (--angle)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.Angle),
            "Or Custom Angle in Degrees (--angle)",
            state.Angle,
            "e.g. 45, 90, 180"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedAngle = FormElementHelper.GetString(formInput, "SelectedAngle");
        string manualAngle = FormElementHelper.GetString(formInput, nameof(state.Angle));

        if (!string.IsNullOrWhiteSpace(manualAngle) &&
            !string.Equals(manualAngle, selectedAngle, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualAngle, state.Angle, StringComparison.OrdinalIgnoreCase))
        {
            state.Angle = manualAngle;
        }
        else if (!string.IsNullOrWhiteSpace(selectedAngle))
        {
            state.Angle = selectedAngle;
        }
        else
        {
            state.Angle = manualAngle;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.Angle))
        {
            args.Add($"--angle={state.Angle}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.Angle = string.Empty;
    }
}