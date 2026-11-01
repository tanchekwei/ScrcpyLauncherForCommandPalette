// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class ForceAdbForward : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.force-adb-forward",
            Title = "Force ADB Forward",
            Arguments = "--force-adb-forward",
            Description = "Force forward connection instead of reverse socket redirection",
            Category = "Connection",
            Icon = Icon.Cast,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/tunnels.md#ssh-tunnel",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.ForceAdbForward),
            "Force ADB Forward (--force-adb-forward)",
            state.ForceAdbForward);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.ForceAdbForward = FormElementHelper.GetBool(formInput, nameof(state.ForceAdbForward));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.ForceAdbForward)
        {
            args.Add("--force-adb-forward");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.ForceAdbForward = false;
    }
}