// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioEncoder : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-encoder",
            Title = "Audio Encoder (--audio-encoder)",
            Arguments = "--audio-encoder=name",
            Description = "Select a specific audio MediaCodec encoder (from --list-encoders)",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#encoder",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var container = new JsonObject
        {
            ["type"] = "Container",
            ["items"] = new JsonArray(),
        };
        var items = (JsonArray)container["items"]!;

        var choices = new List<(string Title, string Value)>
        {
            ("(Default / Custom)", ""),
            ("c2.android.opus.encoder (Android Software Opus)", "c2.android.opus.encoder"),
            ("c2.android.aac.encoder (Android Software AAC)", "c2.android.aac.encoder"),
            ("OMX.google.opus.encoder (Google Software Opus)", "OMX.google.opus.encoder"),
            ("OMX.google.aac.encoder (Google Software AAC)", "OMX.google.aac.encoder"),
            ("c2.android.flac.encoder (Android Software FLAC)", "c2.android.flac.encoder"),
            ("c2.android.raw.encoder (Android Software RAW PCM)", "c2.android.raw.encoder"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.AudioEncoder, StringComparison.OrdinalIgnoreCase))
            ? (state.AudioEncoder ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedAudioEncoder",
            "Select Audio Encoder Preset (--audio-encoder)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.AudioEncoder),
            "Or Custom Audio Encoder (--audio-encoder)",
            state.AudioEncoder,
            "e.g. c2.android.opus.encoder"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedEncoder = FormElementHelper.GetString(formInput, "SelectedAudioEncoder");
        string manualEncoder = FormElementHelper.GetString(formInput, nameof(state.AudioEncoder));

        if (!string.IsNullOrWhiteSpace(manualEncoder) &&
            !string.Equals(manualEncoder, selectedEncoder, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualEncoder, state.AudioEncoder, StringComparison.OrdinalIgnoreCase))
        {
            state.AudioEncoder = manualEncoder;
        }
        else if (!string.IsNullOrWhiteSpace(selectedEncoder))
        {
            state.AudioEncoder = selectedEncoder;
        }
        else
        {
            state.AudioEncoder = manualEncoder;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioEncoder))
        {
            args.Add($"--audio-encoder={state.AudioEncoder}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioEncoder = string.Empty;
    }
}