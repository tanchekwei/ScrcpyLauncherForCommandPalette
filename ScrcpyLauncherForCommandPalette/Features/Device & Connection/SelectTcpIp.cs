// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class SelectTcpIp : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.select-tcpip",
            Title = "Select TCP/IP Device",
            Arguments = "--select-tcpip",
            Description = "Explicitly use the device connected over TCP/IP (-e)",
            Category = "Connection",
            Icon = Icon.Cast,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/connection.md#selection",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.SelectTcpIp),
            "Select TCP/IP Device (-e, --select-tcpip)",
            state.SelectTcpIp);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.SelectTcpIp = FormElementHelper.GetBool(formInput, nameof(state.SelectTcpIp));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.SelectTcpIp)
        {
            args.Add("-e");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.SelectTcpIp = false;
    }
}