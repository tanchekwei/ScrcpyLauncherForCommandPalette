// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class TcpIp : IScrcpyFeature
{
    public const string AutoValue = "auto";

    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.tcpip-auto",
            Title = "Connect via TCP/IP (Wireless Auto)",
            Arguments = "--tcpip",
            Description = "Automatically discover device IP, enable TCP/IP mode and connect wirelessly",
            Category = "Connection",
            Icon = Icon.Cast,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/connection.md#tcpip-wireless",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.tcpip-custom",
            Title = "Connect via TCP/IP (IP Address)",
            Arguments = "--tcpip=192.168.1.1:5555",
            Description = "Connect directly to a specific IP address and port over TCP/IP",
            Category = "Connection",
            Icon = Icon.Cast,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/connection.md#tcpip-wireless",
        },
    ];

    public static bool IsAuto(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var trimmed = value.Trim();
        return string.Equals(trimmed, AutoValue, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "true", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "default", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "--tcpip", StringComparison.OrdinalIgnoreCase);
    }

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
            ("Disabled (Default USB / Serial)", ""),
            ("Auto Discover IP & Connect Wireless (--tcpip)", AutoValue),
            ("(Custom IP:Port Below)", "custom"),
        };

        bool isAuto = IsAuto(state.TcpIp);

        string selectedChoice;
        if (string.IsNullOrWhiteSpace(state.TcpIp))
        {
            selectedChoice = "";
        }
        else if (isAuto)
        {
            selectedChoice = AutoValue;
        }
        else
        {
            selectedChoice = "custom";
        }

        string textValue = (isAuto || string.IsNullOrWhiteSpace(state.TcpIp))
            ? string.Empty
            : state.TcpIp;

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedTcpIp",
            "TCP/IP Wireless Connection (--tcpip)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.TcpIp),
            "Or Custom IP Address & Port (--tcpip=<ip:port>)",
            textValue,
            "e.g. 192.168.1.100 or 192.168.1.100:5555"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedMode = FormElementHelper.GetString(formInput, "SelectedTcpIp");
        string manualIp = FormElementHelper.GetString(formInput, nameof(state.TcpIp)).Trim();

        if (manualIp.StartsWith("--tcpip=", StringComparison.OrdinalIgnoreCase))
        {
            manualIp = manualIp["--tcpip=".Length..].Trim();
        }

        if (!string.IsNullOrWhiteSpace(manualIp))
        {
            if (string.Equals(selectedMode, "custom", StringComparison.OrdinalIgnoreCase) ||
                (!string.Equals(manualIp, state.TcpIp, StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(manualIp, selectedMode, StringComparison.OrdinalIgnoreCase)))
            {
                state.TcpIp = IsAuto(manualIp) ? AutoValue : manualIp;
                return;
            }
        }

        if (string.Equals(selectedMode, AutoValue, StringComparison.OrdinalIgnoreCase))
        {
            state.TcpIp = AutoValue;
        }
        else if (string.Equals(selectedMode, "custom", StringComparison.OrdinalIgnoreCase))
        {
            state.TcpIp = string.IsNullOrWhiteSpace(manualIp) ? string.Empty : (IsAuto(manualIp) ? AutoValue : manualIp);
        }
        else
        {
            // selectedMode is "" (Disabled)
            if (!string.IsNullOrWhiteSpace(manualIp) && !string.Equals(manualIp, state.TcpIp, StringComparison.OrdinalIgnoreCase))
            {
                state.TcpIp = IsAuto(manualIp) ? AutoValue : manualIp;
            }
            else
            {
                state.TcpIp = string.Empty;
            }
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (IsAuto(state.TcpIp))
        {
            args.Add("--tcpip");
        }
        else if (!string.IsNullOrWhiteSpace(state.TcpIp))
        {
            var val = state.TcpIp.Trim();
            if (val.StartsWith("--tcpip=", StringComparison.OrdinalIgnoreCase))
            {
                args.Add(val);
            }
            else if (val.Equals("--tcpip", StringComparison.OrdinalIgnoreCase))
            {
                args.Add("--tcpip");
            }
            else
            {
                args.Add($"--tcpip={val}");
            }
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.TcpIp = string.Empty;
    }
}