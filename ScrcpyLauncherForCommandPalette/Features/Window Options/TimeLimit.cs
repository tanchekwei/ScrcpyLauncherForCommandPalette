// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class TimeLimit : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.time-limit",
            Title = "Time Limit (--time-limit)",
            Arguments = "--time-limit=60",
            Description = "Automatically terminate scrcpy session after specified seconds",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/recording.md#time-limit",
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
            ("30 Seconds", "30"),
            ("1 Minute (60s)", "60"),
            ("2 Minutes (120s)", "120"),
            ("5 Minutes (300s)", "300"),
            ("10 Minutes (600s)", "600"),
            ("30 Minutes (1800s)", "1800"),
            ("1 Hour (3600s)", "3600"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.TimeLimit, StringComparison.OrdinalIgnoreCase))
            ? (state.TimeLimit ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedTimeLimit",
            "Select Time Limit (--time-limit)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.TimeLimit),
            "Or Custom Time in Seconds (--time-limit)",
            state.TimeLimit,
            "e.g. 60, 300, 3600"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedTime = FormElementHelper.GetString(formInput, "SelectedTimeLimit");
        string manualTime = FormElementHelper.GetString(formInput, nameof(state.TimeLimit));

        if (!string.IsNullOrWhiteSpace(manualTime) &&
            !string.Equals(manualTime, selectedTime, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualTime, state.TimeLimit, StringComparison.OrdinalIgnoreCase))
        {
            state.TimeLimit = manualTime;
        }
        else if (!string.IsNullOrWhiteSpace(selectedTime))
        {
            state.TimeLimit = selectedTime;
        }
        else
        {
            state.TimeLimit = manualTime;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.TimeLimit))
        {
            args.Add($"--time-limit={state.TimeLimit}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.TimeLimit = string.Empty;
    }
}