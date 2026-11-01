// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VirtualDisplay;

public sealed class NoVdDestroyContent : IScrcpyFeature
{
    public string Category => "Virtual Display";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoVdDestroyContent),
            "Keep content alive when closed (--no-vd-destroy-content)",
            state.NoVdDestroyContent);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoVdDestroyContent = FormElementHelper.GetBool(formInput, nameof(state.NoVdDestroyContent));
        if (state.NoVdDestroyContent && string.IsNullOrWhiteSpace(state.NewDisplay))
        {
            state.NewDisplay = NewDisplay.AutoResolutionValue;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.NewDisplay) && state.NoVdDestroyContent)
        {
            args.Add("--no-vd-destroy-content");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoVdDestroyContent = false;
    }
}