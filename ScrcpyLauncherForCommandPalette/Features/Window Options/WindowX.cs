// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.WindowOptions;

public sealed class WindowX : IScrcpyFeature
{
	public string Category => "Window Options";

	public IReadOnlyList<ScrcpyCliCommand> Commands =>
	[
		new ScrcpyCliCommand
		{
			Id = "scrcpy.window-x",
			Title = "Window X Position (--window-x)",
			Arguments = "--window-x=100",
			Description = "Set initial horizontal position of the window",
			Category = "Window",
			Icon = Icon.Window,
			DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/window.md#position-and-size",
		},
	];

	public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
	{
		return FormElementHelper.CreateTextInput(
			nameof(state.WindowX),
			"Window X Position (--window-x)",
			state.WindowX,
			"e.g. 100");
	}

	public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
	{
		state.WindowX = FormElementHelper.GetString(formInput, nameof(state.WindowX));
	}

	public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
	{
		if (!string.IsNullOrWhiteSpace(state.WindowX))
		{
			args.Add($"--window-x={state.WindowX.Trim()}");
		}
	}

	public void ResetState(ScrcpyCommandBuilderState state)
	{
		state.WindowX = string.Empty;
	}
}