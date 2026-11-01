// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class KillAdbOnClose : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.kill-adb-on-close",
            Title = "Kill ADB On Exit (--kill-adb-on-close)",
            Arguments = "--kill-adb-on-close",
            Description = "Automatically terminate ADB daemon process when scrcpy window closes",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.KillAdbOnClose),
            "Kill ADB Server On Close (--kill-adb-on-close)",
            state.KillAdbOnClose);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.KillAdbOnClose = FormElementHelper.GetBool(formInput, nameof(state.KillAdbOnClose));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.KillAdbOnClose)
        {
            args.Add("--kill-adb-on-close");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.KillAdbOnClose = false;
    }
}