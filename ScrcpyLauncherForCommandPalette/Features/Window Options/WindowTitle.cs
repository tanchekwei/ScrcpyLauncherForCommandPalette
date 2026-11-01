// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class WindowTitle : IScrcpyFeature
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
            ("(Default / Custom)", ""),
            ("Scrcpy", "Scrcpy"),
            ("Android Device", "Android Device"),
            ("Mobile Screen", "Mobile Screen"),
            ("Phone Mirror", "Phone Mirror"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.WindowTitle, StringComparison.OrdinalIgnoreCase))
            ? (state.WindowTitle ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedWindowTitle",
            "Select Window Title (--window-title)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.WindowTitle),
            "Or Custom Window Title (--window-title)",
            state.WindowTitle,
            "e.g. My Phone"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedTitle = FormElementHelper.GetString(formInput, "SelectedWindowTitle");
        string manualTitle = FormElementHelper.GetString(formInput, nameof(state.WindowTitle));

        if (!string.IsNullOrWhiteSpace(manualTitle) &&
            !string.Equals(manualTitle, selectedTitle, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualTitle, state.WindowTitle, StringComparison.OrdinalIgnoreCase))
        {
            state.WindowTitle = manualTitle;
        }
        else if (!string.IsNullOrWhiteSpace(selectedTitle))
        {
            state.WindowTitle = selectedTitle;
        }
        else
        {
            state.WindowTitle = manualTitle;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.WindowTitle))
        {
            args.Add($"--window-title \"{state.WindowTitle.Trim()}\"");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.WindowTitle = string.Empty;
    }
}