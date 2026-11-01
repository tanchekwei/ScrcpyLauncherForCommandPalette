// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.VideoAndPerformance;

public sealed class NoMipmaps : IScrcpyFeature
{
    public string Category => "Video & Performance";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-mipmaps",
            Title = "Disable Mipmaps (--no-mipmaps)",
            Arguments = "--no-mipmaps",
            Description = "Disable OpenGL mipmap generation to save GPU resources",
            Category = "Video",
            Icon = Icon.Video,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/video.md",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoMipmaps),
            "Disable OpenGL Mipmaps (--no-mipmaps)",
            state.NoMipmaps);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoMipmaps = FormElementHelper.GetBool(formInput, nameof(state.NoMipmaps));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoMipmaps)
        {
            args.Add("--no-mipmaps");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoMipmaps = false;
    }
}