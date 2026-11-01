// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class MinSizeAlignment : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.min-size-alignment",
            Title = "Size Alignment (--min-size-alignment)",
            Arguments = "--min-size-alignment=8",
            Description = "Enforce width and height to be multiples of 1, 2, 4, 8, or 16",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#size",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("Default (1)", ""),
            ("2", "2"),
            ("4", "4"),
            ("8", "8"),
            ("16", "16"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.MinSizeAlignment),
            "Size Alignment (--min-size-alignment)",
            choices,
            state.MinSizeAlignment);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.MinSizeAlignment = FormElementHelper.GetString(formInput, nameof(state.MinSizeAlignment));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.MinSizeAlignment))
        {
            args.Add($"--min-size-alignment={state.MinSizeAlignment}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.MinSizeAlignment = string.Empty;
    }
}