// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VirtualDisplay;

public sealed class FlexDisplay : IScrcpyFeature
{
    public string Category => "Virtual Display";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.flex-display",
            Title = "Flex Virtual Display",
            Arguments = "--new-display --flex-display",
            Description = "Create a virtual display that automatically resizes with the window (-x)",
            Category = "Virtual Display",
            Icon = Icon.Display,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/virtual-display.md#flex-display",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.flex-display-settings",
            Title = "Virtual Display: Settings App",
            Arguments = "--new-display=1024x768 --flex-display --start-app=com.android.settings",
            Description = "Open Android Settings in a resizable flex virtual display",
            Category = "Virtual Display",
            Icon = Icon.Display,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/virtual-display.md#flex-display",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.FlexDisplay),
            "Flex Virtual Display (--flex-display, -x)",
            state.FlexDisplay);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.FlexDisplay = FormElementHelper.GetBool(formInput, nameof(state.FlexDisplay));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.NewDisplay) && state.FlexDisplay)
        {
            args.Add("--flex-display");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.FlexDisplay = false;
    }
}