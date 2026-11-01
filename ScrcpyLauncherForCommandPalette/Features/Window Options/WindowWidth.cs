// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class WindowWidth : IScrcpyFeature
{
  public string Category => "Window Options";

  public IReadOnlyList<ScrcpyCliCommand> Commands =>
  [
      new ScrcpyCliCommand
        {
            Id = "scrcpy.window-width",
            Title = "Window Width (--window-width)",
            Arguments = "--window-width=800",
            Description = "Set initial width of the window",
            Category = "Window",
            Icon = Icon.Window,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#position-and-size",
        },
    ];

  public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
  {
    return FormElementHelper.CreateTextInput(
        nameof(state.WindowWidth),
        "Window Width (--window-width)",
        state.WindowWidth,
        "e.g. 800");
  }

  public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
  {
    state.WindowWidth = FormElementHelper.GetString(formInput, nameof(state.WindowWidth));
  }

  public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
  {
    if (!string.IsNullOrWhiteSpace(state.WindowWidth))
    {
      args.Add($"--window-width={state.WindowWidth.Trim()}");
    }
  }

  public void ResetState(ScrcpyCommandBuilderState state)
  {
    state.WindowWidth = string.Empty;
  }
}