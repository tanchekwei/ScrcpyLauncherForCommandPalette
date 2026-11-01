// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class NoWindowAspectRatioLock : IScrcpyFeature
{
    public string Category => "Window Options";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-aspect-ratio-lock",
            Title = "Free Resizing (No Aspect Ratio Lock)",
            Arguments = "--no-window-aspect-ratio-lock",
            Description = "Allow freeform window resizing without locking aspect ratio",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#position-and-size",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoWindowAspectRatioLock),
            "Free Resizing (--no-window-aspect-ratio-lock)",
            state.NoWindowAspectRatioLock);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoWindowAspectRatioLock = FormElementHelper.GetBool(formInput, nameof(state.NoWindowAspectRatioLock));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoWindowAspectRatioLock)
        {
            args.Add("--no-window-aspect-ratio-lock");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoWindowAspectRatioLock = false;
    }
}