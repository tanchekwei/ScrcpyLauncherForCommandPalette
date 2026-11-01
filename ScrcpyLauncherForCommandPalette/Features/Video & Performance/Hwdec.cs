// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class Hwdec : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.hwdec-auto",
            Title = "Hardware Decoding (Auto)",
            Arguments = "--hwdec=auto",
            Description = "Use hardware decoding if possible (scrcpy 5.0+ default)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#hardware-decoding",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.hwdec-d3d11va",
            Title = "Hardware Decoding (D3D11VA)",
            Arguments = "--hwdec=d3d11va",
            Description = "Hardware decoding using Direct3D 11 Video Acceleration (Windows)",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#hardware-decoding",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.hwdec-disabled",
            Title = "Disable Hardware Decoding",
            Arguments = "--hwdec=disabled",
            Description = "Force software decoding on computer",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#hardware-decoding",
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
            ("Default (Auto if supported)", ""),
            ("Auto (--hwdec=auto)", "auto"),
            ("Disabled / Software only (--hwdec=disabled)", "disabled"),
            ("Direct3D 11 / Windows (--hwdec=d3d11va)", "d3d11va"),
            ("VA-API / Linux (--hwdec=vaapi)", "vaapi"),
            ("VideoToolbox / macOS (--hwdec=videotoolbox)", "videotoolbox"),
            ("(Custom Decoder Below)", "custom"),
        };

        bool isPreset = choices.Any(c => !string.IsNullOrEmpty(c.Value) &&
                                         c.Value != "custom" &&
                                         string.Equals(c.Value, state.Hwdec, StringComparison.OrdinalIgnoreCase));

        string selectedChoice;
        if (string.IsNullOrWhiteSpace(state.Hwdec))
        {
            selectedChoice = "";
        }
        else if (isPreset)
        {
            selectedChoice = state.Hwdec.ToLowerInvariant();
        }
        else
        {
            selectedChoice = "custom";
        }

        string textValue = (string.IsNullOrWhiteSpace(state.Hwdec) || isPreset)
            ? string.Empty
            : state.Hwdec;

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedHwdec",
            "Hardware Decoding (--hwdec=)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.Hwdec),
            "Or Custom Decoder Name (--hwdec=<value>)",
            textValue,
            "e.g. d3d11va, vaapi, disabled, dxva2"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedDecoder = FormElementHelper.GetString(formInput, "SelectedHwdec");
        string manualDecoder = FormElementHelper.GetString(formInput, nameof(state.Hwdec)).Trim();

        if (manualDecoder.StartsWith("--hwdec=", StringComparison.OrdinalIgnoreCase))
        {
            manualDecoder = manualDecoder["--hwdec=".Length..].Trim();
        }

        if (!string.IsNullOrWhiteSpace(manualDecoder))
        {
            if (string.Equals(selectedDecoder, "custom", StringComparison.OrdinalIgnoreCase) ||
                (!string.Equals(manualDecoder, state.Hwdec, StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(manualDecoder, selectedDecoder, StringComparison.OrdinalIgnoreCase)))
            {
                state.Hwdec = manualDecoder;
                return;
            }
        }

        if (string.Equals(selectedDecoder, "custom", StringComparison.OrdinalIgnoreCase))
        {
            state.Hwdec = manualDecoder;
        }
        else if (!string.IsNullOrWhiteSpace(selectedDecoder))
        {
            state.Hwdec = selectedDecoder;
        }
        else
        {
            // selectedDecoder is "" (Default)
            if (!string.IsNullOrWhiteSpace(manualDecoder) && !string.Equals(manualDecoder, state.Hwdec, StringComparison.OrdinalIgnoreCase))
            {
                state.Hwdec = manualDecoder;
            }
            else
            {
                state.Hwdec = string.Empty;
            }
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.Hwdec))
        {
            var val = state.Hwdec.Trim();
            if (val.StartsWith("--hwdec=", StringComparison.OrdinalIgnoreCase))
            {
                args.Add(val);
            }
            else
            {
                args.Add($"--hwdec={val}");
            }
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.Hwdec = string.Empty;
    }
}