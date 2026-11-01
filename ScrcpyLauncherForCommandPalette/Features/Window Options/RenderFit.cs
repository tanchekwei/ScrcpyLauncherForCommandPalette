// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class RenderFit : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.RenderFit),
            "Render Fit Mode (--render-fit=)",
            [
                ("Default (Letterbox)", ""),
                ("Letterbox", "letterbox"),
                ("Unscaled (Flex Displays)", "unscaled"),
                ("Stretched (Fill Window)", "stretched"),
            ],
            state.RenderFit);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RenderFit = FormElementHelper.GetString(formInput, nameof(state.RenderFit));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.RenderFit))
        {
            args.Add($"--render-fit={state.RenderFit.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RenderFit = string.Empty;
    }
}