// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.CustomArguments;

public sealed class ExtraArgs : IScrcpyFeature
{
    public string Category => "Custom Arguments";

    public IReadOnlyList<ScrcpyCliCommand> Commands => [];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateTextInput(
            nameof(state.ExtraArgs),
            "Additional Custom Flags",
            state.ExtraArgs,
            "e.g. --tunnel-host=192.168.1.10");
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.ExtraArgs = FormElementHelper.GetString(formInput, nameof(state.ExtraArgs));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.ExtraArgs))
        {
            args.Add(state.ExtraArgs.Trim());
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.ExtraArgs = string.Empty;
    }
}