// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class BackgroundColor : IScrcpyFeature
{
    public string Category => "Window Options";

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
            ("Black (#000000)", "#000000"),
            ("Dark Gray (#1E1E1E)", "#1E1E1E"),
            ("Gray (#808080)", "#808080"),
            ("Light Gray (#F0F0F0)", "#F0F0F0"),
            ("White (#FFFFFF)", "#FFFFFF"),
            ("Blue (#0055FF)", "#0055FF"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.BackgroundColor, StringComparison.OrdinalIgnoreCase))
            ? (state.BackgroundColor ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedBackgroundColor",
            "Select Background Color (--background-color=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.BackgroundColor),
            "Or Custom Color Hex (--background-color=)",
            state.BackgroundColor,
            "e.g. #000000 or #234567"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedColor = FormElementHelper.GetString(formInput, "SelectedBackgroundColor");
        string manualColor = FormElementHelper.GetString(formInput, nameof(state.BackgroundColor));

        if (!string.IsNullOrWhiteSpace(manualColor) &&
            !string.Equals(manualColor, selectedColor, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualColor, state.BackgroundColor, StringComparison.OrdinalIgnoreCase))
        {
            state.BackgroundColor = manualColor;
        }
        else if (!string.IsNullOrWhiteSpace(selectedColor))
        {
            state.BackgroundColor = selectedColor;
        }
        else
        {
            state.BackgroundColor = manualColor;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.BackgroundColor))
        {
            args.Add($"--background-color={state.BackgroundColor.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.BackgroundColor = string.Empty;
    }
}