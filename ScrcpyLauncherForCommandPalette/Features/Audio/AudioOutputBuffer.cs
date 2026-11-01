// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.Audio;

public sealed class AudioOutputBuffer : IScrcpyFeature
{
    public string Category => "Audio";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.audio-output-buffer",
            Title = "Audio Output Buffer (--audio-output-buffer)",
            Arguments = "--audio-output-buffer=50",
            Description = "Configure audio output buffer delay in milliseconds (default: 50)",
            Category = "Audio",
            Icon = Icon.Audio,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/audio.md#buffering",
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
            ("5ms (Ultra Low Latency)", "5"),
            ("10ms (Low Latency)", "10"),
            ("20ms (Reduced Latency)", "20"),
            ("50ms (Default)", "50"),
            ("100ms (High Latency / Smooth)", "100"),
            ("200ms (Max Stability)", "200"),
        };

        string selectedChoice = choices.Any(c => string.Equals(c.Value, state.AudioOutputBuffer, StringComparison.OrdinalIgnoreCase))
            ? (state.AudioOutputBuffer ?? string.Empty)
            : "";

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedAudioOutputBuffer",
            "Select Audio Output Buffer (--audio-output-buffer)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.AudioOutputBuffer),
            "Or Custom Buffer in ms (--audio-output-buffer)",
            state.AudioOutputBuffer,
            "e.g. 5, 20, 50"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedBuffer = FormElementHelper.GetString(formInput, "SelectedAudioOutputBuffer");
        string manualBuffer = FormElementHelper.GetString(formInput, nameof(state.AudioOutputBuffer));

        if (!string.IsNullOrWhiteSpace(manualBuffer) &&
            !string.Equals(manualBuffer, selectedBuffer, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(manualBuffer, state.AudioOutputBuffer, StringComparison.OrdinalIgnoreCase))
        {
            state.AudioOutputBuffer = manualBuffer;
        }
        else if (!string.IsNullOrWhiteSpace(selectedBuffer))
        {
            state.AudioOutputBuffer = selectedBuffer;
        }
        else
        {
            state.AudioOutputBuffer = manualBuffer;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!state.NoAudio && !string.IsNullOrWhiteSpace(state.AudioOutputBuffer))
        {
            args.Add($"--audio-output-buffer={state.AudioOutputBuffer}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.AudioOutputBuffer = string.Empty;
    }
}