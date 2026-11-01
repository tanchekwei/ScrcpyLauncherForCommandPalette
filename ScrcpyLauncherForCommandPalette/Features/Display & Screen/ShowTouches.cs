// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class ShowTouches : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.show-touches",
            Title = "Show Physical Touches",
            Arguments = "--show-touches",
            Description = "Show physical touches on screen for presentations (-t)",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#show-touches",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.ShowTouches),
            "Show Physical Touches on screen (-t, --show-touches)",
            state.ShowTouches);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.ShowTouches = FormElementHelper.GetBool(formInput, nameof(state.ShowTouches));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.ShowTouches)
        {
            args.Add("--show-touches");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.ShowTouches = false;
    }
}