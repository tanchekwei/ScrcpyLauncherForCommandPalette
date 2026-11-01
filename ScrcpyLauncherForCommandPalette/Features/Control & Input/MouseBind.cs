// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class MouseBind : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.mouse-bind",
            Title = "Mouse Click Bindings (--mouse-bind)",
            Arguments = "--mouse-bind=bhsn:++++",
            Description = "Configure secondary click shortcuts (right, middle, 4th, 5th click)",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/mouse.md#mouse-bindings",
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
            ("Default: Back/Home/Switch/Notif (bhsn:++++)", "bhsn:++++"),
            ("Forward All Mouse Clicks to Device (++++:++++)", "++++:++++"),
            ("Standard Secondary Shortcuts Only (bhsn)", "bhsn"),
            ("Forward Right Click, Keep Others (+hsn:b+++)", "+hsn:b+++"),
            ("Forward Right & Middle Click (++sn:bh++)", "++sn:bh++"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.MouseBind, StringComparison.OrdinalIgnoreCase))
            ? (state.MouseBind ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedMouseBind",
            "Select Mouse Bindings Preset (--mouse-bind)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.MouseBind),
            "Or Custom Mouse Bindings (--mouse-bind)",
            state.MouseBind,
            "e.g. bhsn:++++, ++++:bhsn"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedBind = FormElementHelper.GetString(formInput, "SelectedMouseBind");
        string manualBind = FormElementHelper.GetString(formInput, nameof(state.MouseBind));

        if (!string.IsNullOrWhiteSpace(manualBind) &&
            !string.Equals(manualBind, selectedBind, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualBind, state.MouseBind, StringComparison.OrdinalIgnoreCase))
        {
            state.MouseBind = manualBind;
        }
        else if (!string.IsNullOrWhiteSpace(selectedBind))
        {
            state.MouseBind = selectedBind;
        }
        else
        {
            state.MouseBind = manualBind;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.MouseBind))
        {
            args.Add($"--mouse-bind={state.MouseBind}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.MouseBind = string.Empty;
    }
}