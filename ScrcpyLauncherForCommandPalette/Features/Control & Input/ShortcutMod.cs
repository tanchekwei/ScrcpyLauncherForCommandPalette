// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class ShortcutMod : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.shortcut-mod",
            Title = "Shortcut Modifiers (--shortcut-mod)",
            Arguments = "--shortcut-mod=lctrl,lsuper",
            Description = "Customize scrcpy shortcut modifier keys (e.g. lctrl, rctrl, lalt, ralt, lsuper)",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/shortcuts.md",
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
            ("Left Alt or Left Super (lalt,lsuper - Default)", "lalt,lsuper"),
            ("Left Control or Right Control (lctrl,rctrl)", "lctrl,rctrl"),
            ("Left Control (lctrl)", "lctrl"),
            ("Right Control (rctrl)", "rctrl"),
            ("Left Alt (lalt)", "lalt"),
            ("Right Alt (ralt)", "ralt"),
            ("Left Super / Windows Key (lsuper)", "lsuper"),
            ("Right Super (rsuper)", "rsuper"),
            ("Left Alt or Right Alt (lalt,ralt)", "lalt,ralt"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.ShortcutMod, StringComparison.OrdinalIgnoreCase))
            ? (state.ShortcutMod ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedShortcutMod",
            "Select Shortcut Modifier Preset (--shortcut-mod)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.ShortcutMod),
            "Or Custom Shortcut Modifier Keys (--shortcut-mod)",
            state.ShortcutMod,
            "e.g. lalt,lsuper (default) or lctrl,rctrl"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedMod = FormElementHelper.GetString(formInput, "SelectedShortcutMod");
        string manualMod = FormElementHelper.GetString(formInput, nameof(state.ShortcutMod));

        if (!string.IsNullOrWhiteSpace(manualMod) &&
            !string.Equals(manualMod, selectedMod, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualMod, state.ShortcutMod, StringComparison.OrdinalIgnoreCase))
        {
            state.ShortcutMod = manualMod;
        }
        else if (!string.IsNullOrWhiteSpace(selectedMod))
        {
            state.ShortcutMod = selectedMod;
        }
        else
        {
            state.ShortcutMod = manualMod;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.ShortcutMod))
        {
            args.Add($"--shortcut-mod={state.ShortcutMod}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.ShortcutMod = string.Empty;
    }
}