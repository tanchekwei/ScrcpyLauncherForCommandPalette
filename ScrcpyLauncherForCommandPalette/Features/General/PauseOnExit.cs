// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.General;

public sealed class PauseOnExit : IScrcpyFeature
{
    public string Category => "General";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.pause-on-exit",
            Title = "Pause On Exit (--pause-on-exit)",
            Arguments = "--pause-on-exit=if-error",
            Description = "Keep console window open after scrcpy terminates to view error logs",
            Category = "General",
            Icon = Icon.Terminal,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/windows.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("Disabled (default)", ""),
            ("Only on Error (if-error)", "if-error"),
            ("Always Pause (true)", "true"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.PauseOnExit),
            "Pause Terminal On Exit (--pause-on-exit)",
            choices,
            state.PauseOnExit);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.PauseOnExit = FormElementHelper.GetString(formInput, nameof(state.PauseOnExit));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.PauseOnExit))
        {
            args.Add($"--pause-on-exit={state.PauseOnExit}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.PauseOnExit = string.Empty;
    }
}