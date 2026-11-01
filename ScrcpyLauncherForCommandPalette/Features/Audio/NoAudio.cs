// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class NoAudio : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-audio",
            Title = "No Audio",
            Arguments = "--no-audio",
            Description = "Mirror display without capturing or forwarding audio",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#no-audio",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoAudio),
            "Disable Audio Forwarding (--no-audio)",
            state.NoAudio);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoAudio = FormElementHelper.GetBool(formInput, nameof(state.NoAudio));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoAudio)
        {
            args.Add("--no-audio");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoAudio = false;
    }
}