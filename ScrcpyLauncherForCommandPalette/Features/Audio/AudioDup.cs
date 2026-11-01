// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioDup : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-dup",
            Title = "Duplicate Audio (Android 13+)",
            Arguments = "--audio-dup",
            Description = "Play audio on both the computer and the device simultaneously",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#duplication",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.AudioDup),
            "Duplicate Audio to PC & Device (--audio-dup, Android 13+)",
            state.AudioDup);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.AudioDup = FormElementHelper.GetBool(formInput, nameof(state.AudioDup));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && state.AudioDup)
        {
            args.Add("--audio-dup");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioDup = false;
    }
}