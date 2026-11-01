// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class WindowY : IScrcpyFeature
{
	public string Category => "Window Options";

	public IReadOnlyList<ScrcpyCliCommand> Commands =>
	[
		new ScrcpyCliCommand
		{
			Id = "scrcpy.window-y",
			Title = "Window Y Position (--window-y)",
			Arguments = "--window-y=100",
			Description = "Set initial vertical position of the window",
			Category = "Window",
			Icon = Icon.Window,
			DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#position-and-size",
		},
	];

	public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
	{
		return FormElementHelper.CreateTextInput(
			nameof(state.WindowY),
			"Window Y Position (--window-y)",
			state.WindowY,
			"e.g. 100");
	}

	public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
	{
		state.WindowY = FormElementHelper.GetString(formInput, nameof(state.WindowY));
	}

	public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
	{
		if (!string.IsNullOrWhiteSpace(state.WindowY))
		{
			args.Add($"--window-y={state.WindowY.Trim()}");
		}
	}

	public void ResetState(ScrcpyCommandBuilderState state)
	{
		state.WindowY = string.Empty;
	}
}