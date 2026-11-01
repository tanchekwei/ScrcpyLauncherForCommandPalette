// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class PushTarget : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.push-target",
            Title = "File Push Target Directory (--push-target)",
            Arguments = "--push-target=/sdcard/Download/",
            Description = "Set destination folder on device for drag-and-dropped files (default: /sdcard/Download/)",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/control.md#push-file-to-device",
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
            ("Downloads (/sdcard/Download/)", "/sdcard/Download/"),
            ("Movies / Videos (/sdcard/Movies/)", "/sdcard/Movies/"),
            ("Music (/sdcard/Music/)", "/sdcard/Music/"),
            ("Pictures (/sdcard/Pictures/)", "/sdcard/Pictures/"),
            ("DCIM / Camera (/sdcard/DCIM/)", "/sdcard/DCIM/"),
            ("Documents (/sdcard/Documents/)", "/sdcard/Documents/"),
            ("Internal Storage Root (/sdcard/)", "/sdcard/"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.PushTarget, StringComparison.OrdinalIgnoreCase))
            ? (state.PushTarget ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedPushTarget",
            "Select Push Target Preset (--push-target)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.PushTarget),
            "Or Custom Push Target Path (--push-target)",
            state.PushTarget,
            "e.g. /sdcard/Movies/ or /sdcard/Download/"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedTarget = FormElementHelper.GetString(formInput, "SelectedPushTarget");
        string manualTarget = FormElementHelper.GetString(formInput, nameof(state.PushTarget));

        if (!string.IsNullOrWhiteSpace(manualTarget) &&
            !string.Equals(manualTarget, selectedTarget, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualTarget, state.PushTarget, StringComparison.OrdinalIgnoreCase))
        {
            state.PushTarget = manualTarget;
        }
        else if (!string.IsNullOrWhiteSpace(selectedTarget))
        {
            state.PushTarget = selectedTarget;
        }
        else
        {
            state.PushTarget = manualTarget;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.PushTarget))
        {
            args.Add($"--push-target={state.PushTarget}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PushTarget = string.Empty;
    }
}