// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class PortRange : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.port",
            Title = "Local TCP Port (--port)",
            Arguments = "--port=27183:27199",
            Description = "Set the TCP port (or port range) used by scrcpy client to listen",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/tunnels.md",
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
            ("Default Range (27183:27199)", "27183:27199"),
            ("Default Single Port (27183)", "27183"),
            ("Alternative Range (28183:28199)", "28183:28199"),
            ("Alternative Single Port (28183)", "28183"),
            ("ADB Default Port (5555)", "5555"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.PortRange, StringComparison.OrdinalIgnoreCase))
            ? (state.PortRange ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedPortRange",
            "Select TCP Port Preset (--port)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.PortRange),
            "Or Custom TCP Port / Port Range (--port)",
            state.PortRange,
            "e.g. 27183 or 27183:27199"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedPort = FormElementHelper.GetString(formInput, "SelectedPortRange");
        string manualPort = FormElementHelper.GetString(formInput, nameof(state.PortRange));

        if (!string.IsNullOrWhiteSpace(manualPort) &&
            !string.Equals(manualPort, selectedPort, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualPort, state.PortRange, StringComparison.OrdinalIgnoreCase))
        {
            state.PortRange = manualPort;
        }
        else if (!string.IsNullOrWhiteSpace(selectedPort))
        {
            state.PortRange = selectedPort;
        }
        else
        {
            state.PortRange = manualPort;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.PortRange))
        {
            args.Add($"--port={state.PortRange}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PortRange = string.Empty;
    }
}