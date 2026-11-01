// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VirtualDisplay;

public sealed class NewDisplay : IScrcpyFeature
{
    public const string AutoResolutionValue = "auto";

    public string Category => "Virtual Display";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.new-display",
            Title = "New Virtual Display",
            Arguments = "--new-display",
            Description = "Create and mirror a new secondary virtual display using device resolution",
            Category = "Virtual Display",
            Icon = Icon.Display,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/virtual-display.md#new-display",
        },
        new ScrcpyCliCommand
        {
            Id = "scrcpy.new-display-1080p",
            Title = "New Virtual Display (1080p)",
            Arguments = "--new-display=1920x1080",
            Description = "Create and mirror a new secondary virtual display at 1920x1080",
            Category = "Virtual Display",
            Icon = Icon.Display,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/virtual-display.md#new-display",
        },
    ];

    public static bool IsAuto(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var trimmed = value.Trim();
        return string.Equals(trimmed, AutoResolutionValue, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "device", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "phone", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "true", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "default", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "--new-display", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "<device>", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(trimmed, "<phone>", StringComparison.OrdinalIgnoreCase);
    }

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
            ("(Disabled)", ""),
            ("Device / Phone Resolution (--new-display)", AutoResolutionValue),
            ("1920x1080 (1080p Landscape)", "1920x1080"),
            ("1080x1920 (1080p Portrait)", "1080x1920"),
            ("1280x720 (720p Landscape)", "1280x720"),
            ("720x1280 (720p Portrait)", "720x1280"),
            ("2560x1440 (1440p Landscape)", "2560x1440"),
            ("1440x2560 (1440p Portrait)", "1440x2560"),
            ("3840x2160 (4K Landscape)", "3840x2160"),
            ("2160x3840 (4K Portrait)", "2160x3840"),
            ("(Custom Resolution Below)", "custom"),
        };

        bool isAuto = IsAuto(state.NewDisplay);
        bool isPreset = choices.Any(c => !string.IsNullOrEmpty(c.Value) &&
                                         c.Value != "custom" &&
                                         c.Value != AutoResolutionValue &&
                                         string.Equals(c.Value, state.NewDisplay, StringComparison.OrdinalIgnoreCase));

        string selectedChoice;
        if (string.IsNullOrWhiteSpace(state.NewDisplay))
        {
            selectedChoice = "";
        }
        else if (isAuto)
        {
            selectedChoice = AutoResolutionValue;
        }
        else if (isPreset)
        {
            selectedChoice = state.NewDisplay;
        }
        else
        {
            selectedChoice = "custom";
        }

        string textValue = (isAuto || string.IsNullOrWhiteSpace(state.NewDisplay) || isPreset)
            ? string.Empty
            : state.NewDisplay;

        items.AddNode(FormElementHelper.CreateChoiceSet(
            "SelectedNewDisplay",
            "Select Resolution (--new-display)",
            [.. choices],
            selectedChoice));

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.NewDisplay),
            "Or Custom Resolution (--new-display=<value>)",
            textValue,
            "e.g. 1920x1080, or 1920x1080/420"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string selectedRes = FormElementHelper.GetString(formInput, "SelectedNewDisplay");
        string manualRes = FormElementHelper.GetString(formInput, nameof(state.NewDisplay)).Trim();

        if (manualRes.StartsWith("--new-display=", StringComparison.OrdinalIgnoreCase))
        {
            manualRes = manualRes["--new-display=".Length..].Trim();
        }

        if (!string.IsNullOrWhiteSpace(manualRes))
        {
            if (string.Equals(selectedRes, "custom", StringComparison.OrdinalIgnoreCase) ||
                (!string.Equals(manualRes, state.NewDisplay, StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(manualRes, selectedRes, StringComparison.OrdinalIgnoreCase)))
            {
                state.NewDisplay = IsAuto(manualRes) ? AutoResolutionValue : manualRes;
                return;
            }
        }

        if (string.Equals(selectedRes, AutoResolutionValue, StringComparison.OrdinalIgnoreCase))
        {
            state.NewDisplay = AutoResolutionValue;
        }
        else if (string.Equals(selectedRes, "custom", StringComparison.OrdinalIgnoreCase))
        {
            state.NewDisplay = string.IsNullOrWhiteSpace(manualRes) ? string.Empty : (IsAuto(manualRes) ? AutoResolutionValue : manualRes);
        }
        else if (!string.IsNullOrWhiteSpace(selectedRes))
        {
            state.NewDisplay = selectedRes;
        }
        else
        {
            // selectedRes is "" (Disabled)
            if (!string.IsNullOrWhiteSpace(manualRes) && !string.Equals(manualRes, state.NewDisplay, StringComparison.OrdinalIgnoreCase))
            {
                state.NewDisplay = IsAuto(manualRes) ? AutoResolutionValue : manualRes;
            }
            else
            {
                state.NewDisplay = string.Empty;
            }
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (IsAuto(state.NewDisplay))
        {
            args.Add("--new-display");
        }
        else if (!string.IsNullOrWhiteSpace(state.NewDisplay))
        {
            var val = state.NewDisplay.Trim();
            if (val.StartsWith("--new-display=", StringComparison.OrdinalIgnoreCase))
            {
                args.Add(val);
            }
            else if (val.Equals("--new-display", StringComparison.OrdinalIgnoreCase))
            {
                args.Add("--new-display");
            }
            else
            {
                args.Add($"--new-display={val}");
            }
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NewDisplay = string.Empty;
    }
}