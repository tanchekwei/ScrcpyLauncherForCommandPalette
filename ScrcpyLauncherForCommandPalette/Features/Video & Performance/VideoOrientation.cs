// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class VideoOrientation : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.VideoOrientation),
            "Display Orientation (--orientation=)",
            [
                ("Default", ""),
                ("0°", "0"),
                ("90° (Clockwise)", "90"),
                ("180°", "180"),
                ("270° (Counterclockwise)", "270"),
                ("Flip 0°", "flip0"),
                ("Flip 90°", "flip90"),
                ("Flip 180°", "flip180"),
                ("Flip 270°", "flip270"),
            ],
            state.VideoOrientation);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.VideoOrientation = FormElementHelper.GetString(formInput, nameof(state.VideoOrientation));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoVideo && !string.IsNullOrWhiteSpace(state.VideoOrientation))
        {
            args.Add($"--orientation={state.VideoOrientation.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.VideoOrientation = string.Empty;
    }
}