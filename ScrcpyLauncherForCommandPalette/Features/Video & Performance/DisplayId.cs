// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class DisplayId : IScrcpyFeature
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
            ("(Default / Custom)", ""),
            ("0 (Main / Primary Display)", "0"),
            ("1 (Secondary Display 1)", "1"),
            ("2 (Secondary Display 2)", "2"),
            ("3 (Secondary Display 3)", "3"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.DisplayId, StringComparison.OrdinalIgnoreCase))
            ? (state.DisplayId ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedDisplayId",
            "Select Display ID (--display-id=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.DisplayId),
            "Or Custom Display ID (--display-id=)",
            state.DisplayId,
            "e.g. 0, 1, 2"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedId = FormElementHelper.GetString(formInput, "SelectedDisplayId");
        string manualId = FormElementHelper.GetString(formInput, nameof(state.DisplayId));

        if (!string.IsNullOrWhiteSpace(manualId) &&
            !string.Equals(manualId, selectedId, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualId, state.DisplayId, StringComparison.OrdinalIgnoreCase))
        {
            state.DisplayId = manualId;
        }
        else if (!string.IsNullOrWhiteSpace(selectedId))
        {
            state.DisplayId = selectedId;
        }
        else
        {
            state.DisplayId = manualId;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.DisplayId))
        {
            args.Add($"--display-id={state.DisplayId.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.DisplayId = string.Empty;
    }
}