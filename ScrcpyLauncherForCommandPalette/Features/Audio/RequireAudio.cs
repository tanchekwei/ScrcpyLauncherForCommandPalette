// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class RequireAudio : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.RequireAudio),
            "Fail if Audio Capture Fails (--require-audio)",
            state.RequireAudio);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.RequireAudio = FormElementHelper.GetBool(formInput, nameof(state.RequireAudio));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.RequireAudio)
        {
            args.Add("--require-audio");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.RequireAudio = false;
    }
}