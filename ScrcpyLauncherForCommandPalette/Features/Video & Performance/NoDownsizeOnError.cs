// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class NoDownsizeOnError : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-downsize-on-error",
            Title = "No Downsize On Error (--no-downsize-on-error)",
            Arguments = "--no-downsize-on-error",
            Description = "Disable automatic resolution fallback if video encoder fails",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md#size",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoDownsizeOnError),
            "Disable Resolution Fallback (--no-downsize-on-error)",
            state.NoDownsizeOnError);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoDownsizeOnError = FormElementHelper.GetBool(formInput, nameof(state.NoDownsizeOnError));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoDownsizeOnError)
        {
            args.Add("--no-downsize-on-error");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoDownsizeOnError = false;
    }
}