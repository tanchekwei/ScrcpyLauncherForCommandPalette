// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class NoMouseHover : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-mouse-hover",
            Title = "Disable Mouse Hover (--no-mouse-hover)",
            Arguments = "--no-mouse-hover",
            Description = "Do not forward mouse motion without clicks (hover events)",
            Category = "Control",
            Icon = Icon.Mouse,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/mouse.md#mouse-hover",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoMouseHover),
            "Disable Mouse Hover (--no-mouse-hover)",
            state.NoMouseHover);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoMouseHover = FormElementHelper.GetBool(formInput, nameof(state.NoMouseHover));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoMouseHover)
        {
            args.Add("--no-mouse-hover");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoMouseHover = false;
    }
}