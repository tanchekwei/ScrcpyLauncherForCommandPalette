// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class TunnelPort : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.tunnel-port",
            Title = "Tunnel Port (--tunnel-port)",
            Arguments = "--tunnel-port=27183",
            Description = "Set the TCP port of the adb tunnel to reach the scrcpy server",
            Category = "Connection",
            Icon = Icon.Phone,
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateTextInput(
            nameof(state.TunnelPort),
            "Tunnel Port (--tunnel-port=)",
            state.TunnelPort,
            "e.g. 27183");
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.TunnelPort = FormElementHelper.GetString(formInput, nameof(state.TunnelPort));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.TunnelPort))
        {
            args.Add($"--tunnel-port={state.TunnelPort.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.TunnelPort = string.Empty;
    }
}