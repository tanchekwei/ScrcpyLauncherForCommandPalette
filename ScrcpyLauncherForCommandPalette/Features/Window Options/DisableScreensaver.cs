// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class DisableScreensaver : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.disable-screensaver",
            Title = "Disable Screensaver",
            Arguments = "--disable-screensaver",
            Description = "Prevent PC screensaver from starting while scrcpy is running",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#disable-screensaver",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.DisableScreensaver),
            "Disable Screensaver while running (--disable-screensaver)",
            state.DisableScreensaver);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.DisableScreensaver = FormElementHelper.GetBool(formInput, nameof(state.DisableScreensaver));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.DisableScreensaver)
        {
            args.Add("--disable-screensaver");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.DisableScreensaver = false;
    }
}