// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class NoTerminalTitle : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-terminal-title",
            Title = "Disable Terminal Title (--no-terminal-title)",
            Arguments = "--no-terminal-title",
            Description = "Do not change console window title to device name",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#title",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoTerminalTitle),
            "Disable Terminal Title Updates (--no-terminal-title)",
            state.NoTerminalTitle);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoTerminalTitle = FormElementHelper.GetBool(formInput, nameof(state.NoTerminalTitle));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoTerminalTitle)
        {
            args.Add("--no-terminal-title");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoTerminalTitle = false;
    }
}