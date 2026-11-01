// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class WindowHeight : IScrcpyFeature
{
	public string Category => "Window Options";

	public IReadOnlyList<ScrcpyCliCommand> Commands =>
	[
			new ScrcpyCliCommand
				{
						Id = "scrcpy.window-height",
						Title = "Window Height (--window-height)",
						Arguments = "--window-height=600",
						Description = "Set initial height of the window",
						Category = "Window",
						Icon = Icon.Window,
						DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#position-and-size",
				},
		];

	public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
	{
		return FormElementHelper.CreateTextInput(
				nameof(state.WindowHeight),
				"Window Height (--window-height)",
				state.WindowHeight,
				"e.g. 600");
	}

	public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
	{
		state.WindowHeight = FormElementHelper.GetString(formInput, nameof(state.WindowHeight));
	}

	public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
	{
		if (!string.IsNullOrWhiteSpace(state.WindowHeight))
		{
			args.Add($"--window-height={state.WindowHeight.Trim()}");
		}
	}

	public void ResetState(ScrcpyCommandBuilderState state)
	{
		state.WindowHeight = string.Empty;
	}
}