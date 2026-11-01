// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class TunnelHost : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.tunnel-host",
            Title = "Tunnel Host (--tunnel-host)",
            Arguments = "--tunnel-host=192.168.1.2",
            Description = "Set IP address of ADB tunnel to reach scrcpy server (auto enables --force-adb-forward)",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/tunnels.md#remote-adb-server",
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
            ("Localhost (127.0.0.1)", "127.0.0.1"),
            ("Localhost (localhost)", "localhost"),
            ("Common Gateway (192.168.1.1)", "192.168.1.1"),
            ("Common Gateway (192.168.0.1)", "192.168.0.1"),
            ("Android Emulator Host (10.0.2.2)", "10.0.2.2"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.TunnelHost, StringComparison.OrdinalIgnoreCase))
            ? (state.TunnelHost ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedTunnelHost",
            "Select Tunnel Host Preset (--tunnel-host)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.TunnelHost),
            "Or Custom ADB Tunnel Host IP (--tunnel-host)",
            state.TunnelHost,
            "e.g. 192.168.1.2, localhost"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedHost = FormElementHelper.GetString(formInput, "SelectedTunnelHost");
        string manualHost = FormElementHelper.GetString(formInput, nameof(state.TunnelHost));

        if (!string.IsNullOrWhiteSpace(manualHost) &&
            !string.Equals(manualHost, selectedHost, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualHost, state.TunnelHost, StringComparison.OrdinalIgnoreCase))
        {
            state.TunnelHost = manualHost;
        }
        else if (!string.IsNullOrWhiteSpace(selectedHost))
        {
            state.TunnelHost = selectedHost;
        }
        else
        {
            state.TunnelHost = manualHost;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.TunnelHost))
        {
            args.Add($"--tunnel-host={state.TunnelHost}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.TunnelHost = string.Empty;
    }
}