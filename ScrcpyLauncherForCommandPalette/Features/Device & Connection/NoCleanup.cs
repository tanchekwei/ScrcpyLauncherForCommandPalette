// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DeviceAndConnection;

public sealed class NoCleanup : IScrcpyFeature
{
    public string Category => "Device & Connection";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-cleanup",
            Title = "Disable Cleanup On Exit (--no-cleanup)",
            Arguments = "--no-cleanup",
            Description = "Do not restore device states (screen off, stay awake, touches) when scrcpy closes",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoCleanup),
            "Do Not Restore Device State On Exit (--no-cleanup)",
            state.NoCleanup);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoCleanup = FormElementHelper.GetBool(formInput, nameof(state.NoCleanup));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoCleanup)
        {
            args.Add("--no-cleanup");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoCleanup = false;
    }
}