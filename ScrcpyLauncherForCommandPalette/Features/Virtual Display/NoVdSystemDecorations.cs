// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VirtualDisplay;

public sealed class NoVdSystemDecorations : IScrcpyFeature
{
    public string Category => "Virtual Display";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoVdSystemDecorations),
            "Disable System Decorations (--no-vd-system-decorations)",
            state.NoVdSystemDecorations);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoVdSystemDecorations = FormElementHelper.GetBool(formInput, nameof(state.NoVdSystemDecorations));
        if (state.NoVdSystemDecorations && string.IsNullOrWhiteSpace(state.NewDisplay))
        {
            state.NewDisplay = NewDisplay.AutoResolutionValue;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.NewDisplay) && state.NoVdSystemDecorations)
        {
            args.Add("--no-vd-system-decorations");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoVdSystemDecorations = false;
    }
}