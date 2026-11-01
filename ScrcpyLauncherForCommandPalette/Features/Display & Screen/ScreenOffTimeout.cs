// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class ScreenOffTimeout : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.screen-off-timeout-300",
            Title = "Screen Off Timeout (5 min)",
            Arguments = "--screen-off-timeout=300",
            Description = "Set device display sleep timeout to 300 seconds (5 minutes) while running",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#screen-off-timeout",
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
            ("15 Seconds", "15"),
            ("30 Seconds", "30"),
            ("1 Minute (60s)", "60"),
            ("2 Minutes (120s)", "120"),
            ("5 Minutes (300s)", "300"),
            ("10 Minutes (600s)", "600"),
            ("30 Minutes (1800s)", "1800"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.ScreenOffTimeout, StringComparison.OrdinalIgnoreCase))
            ? (state.ScreenOffTimeout ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedScreenOffTimeout",
            "Select Screen Off Timeout (--screen-off-timeout=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.ScreenOffTimeout),
            "Or Custom Timeout in seconds (--screen-off-timeout=)",
            state.ScreenOffTimeout,
            "e.g. 300 (5 minutes)"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedTimeout = FormElementHelper.GetString(formInput, "SelectedScreenOffTimeout");
        string manualTimeout = FormElementHelper.GetString(formInput, nameof(state.ScreenOffTimeout));

        if (!string.IsNullOrWhiteSpace(manualTimeout) &&
            !string.Equals(manualTimeout, selectedTimeout, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualTimeout, state.ScreenOffTimeout, StringComparison.OrdinalIgnoreCase))
        {
            state.ScreenOffTimeout = manualTimeout;
        }
        else if (!string.IsNullOrWhiteSpace(selectedTimeout))
        {
            state.ScreenOffTimeout = selectedTimeout;
        }
        else
        {
            state.ScreenOffTimeout = manualTimeout;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.ScreenOffTimeout))
        {
            args.Add($"--screen-off-timeout={state.ScreenOffTimeout.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.ScreenOffTimeout = string.Empty;
    }
}