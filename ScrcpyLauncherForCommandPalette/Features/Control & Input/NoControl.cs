// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class NoControl : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-control",
            Title = "Read-Only Mode (No Control)",
            Arguments = "--no-control",
            Description = "Disable keyboard, mouse, and touch interaction (-n)",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/control.md#read-only",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoControl),
            "Read-Only / Disable Control (-n, --no-control)",
            state.NoControl);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoControl = FormElementHelper.GetBool(formInput, nameof(state.NoControl));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoControl)
        {
            args.Add("--no-control");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoControl = false;
    }
}