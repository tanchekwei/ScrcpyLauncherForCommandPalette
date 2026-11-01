// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class Crop : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

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
            ("Top Half (Portrait: 1080:960:0:0)", "1080:960:0:0"),
            ("Bottom Half (Portrait: 1080:960:0:960)", "1080:960:0:960"),
            ("Center Square (Portrait: 1080:1080:0:420)", "1080:1080:0:420"),
            ("Left Half (Landscape: 960:1080:0:0)", "960:1080:0:0"),
            ("Right Half (Landscape: 960:1080:960:0)", "960:1080:960:0"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.Crop, StringComparison.OrdinalIgnoreCase))
            ? (state.Crop ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedCrop",
            "Select Crop Preset (--crop=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.Crop),
            "Or Custom Crop (--crop=width:height:x:y)",
            state.Crop,
            "e.g. 1224:1440:0:0"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedCrop = FormElementHelper.GetString(formInput, "SelectedCrop");
        string manualCrop = FormElementHelper.GetString(formInput, nameof(state.Crop));

        if (!string.IsNullOrWhiteSpace(manualCrop) &&
            !string.Equals(manualCrop, selectedCrop, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualCrop, state.Crop, StringComparison.OrdinalIgnoreCase))
        {
            state.Crop = manualCrop;
        }
        else if (!string.IsNullOrWhiteSpace(selectedCrop))
        {
            state.Crop = selectedCrop;
        }
        else
        {
            state.Crop = manualCrop;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.Crop))
        {
            args.Add($"--crop={state.Crop.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.Crop = string.Empty;
    }
}