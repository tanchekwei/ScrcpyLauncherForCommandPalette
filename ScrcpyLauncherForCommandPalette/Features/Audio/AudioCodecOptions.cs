// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioCodecOptions : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-codec-options",
            Title = "Audio Codec Options (--audio-codec-options)",
            Arguments = "--audio-codec-options=key:type=val",
            Description = "Pass low-level MediaFormat options to the device audio encoder",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#codec-options",
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
            ("(None / Custom)", ""),
            ("flac-compression-level=5 (Default FLAC)", "flac-compression-level=5"),
            ("flac-compression-level=8 (Max FLAC)", "flac-compression-level=8"),
            ("bitrate=128000 (128 kbps)", "bitrate=128000"),
            ("bitrate=192000 (192 kbps)", "bitrate=192000"),
            ("bitrate=256000 (256 kbps)", "bitrate=256000"),
            ("bitrate=320000 (320 kbps)", "bitrate=320000"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.AudioCodecOptions, StringComparison.OrdinalIgnoreCase))
            ? (state.AudioCodecOptions ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedAudioCodecOptions",
            "Select Audio Codec Option Preset (--audio-codec-options)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.AudioCodecOptions),
            "Or Custom Codec Options (--audio-codec-options)",
            state.AudioCodecOptions,
            "e.g. flac-compression-level=8"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedOpt = FormElementHelper.GetString(formInput, "SelectedAudioCodecOptions");
        string manualOpt = FormElementHelper.GetString(formInput, nameof(state.AudioCodecOptions));

        if (!string.IsNullOrWhiteSpace(manualOpt) &&
            !string.Equals(manualOpt, selectedOpt, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualOpt, state.AudioCodecOptions, StringComparison.OrdinalIgnoreCase))
        {
            state.AudioCodecOptions = manualOpt;
        }
        else if (!string.IsNullOrWhiteSpace(selectedOpt))
        {
            state.AudioCodecOptions = selectedOpt;
        }
        else
        {
            state.AudioCodecOptions = manualOpt;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioCodecOptions))
        {
            args.Add($"--audio-codec-options={state.AudioCodecOptions}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioCodecOptions = string.Empty;
    }
}