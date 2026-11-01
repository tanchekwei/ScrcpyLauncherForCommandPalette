// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VirtualDisplay;

public sealed class DisplayImePolicy : IScrcpyFeature
{
    public string Category => "Virtual Display";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.display-local-ime",
            Title = "Virtual Display (Local IME)",
            Arguments = "--new-display --display-ime-policy=local",
            Description = "Virtual display with on-screen keyboard shown on the virtual display itself",
            Category = "Virtual Display",
            Icon = Icon.Display,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/virtual-display.md#display-ime-policy",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateChoiceSet(
            nameof(state.DisplayImePolicy),
            "Display IME Policy (--display-ime-policy=)",
            [
                ("Default", ""),
                ("Local", "local"),
                ("Device", "device"),
            ],
            state.DisplayImePolicy);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.DisplayImePolicy = FormElementHelper.GetString(formInput, nameof(state.DisplayImePolicy));
        if (!string.IsNullOrWhiteSpace(state.DisplayImePolicy) && string.IsNullOrWhiteSpace(state.NewDisplay))
        {
            state.NewDisplay = NewDisplay.AutoResolutionValue;
        }
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.DisplayImePolicy))
        {
            args.Add($"--display-ime-policy={state.DisplayImePolicy.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.DisplayImePolicy = string.Empty;
    }
}