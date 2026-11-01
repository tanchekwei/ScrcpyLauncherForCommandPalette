// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.General;

public sealed class Verbosity : IScrcpyFeature
{
    public string Category => "General";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.verbosity",
            Title = "Log Verbosity (--verbosity / -V)",
            Arguments = "--verbosity=verbose",
            Description = "Set log level (verbose, debug, info, warn, error)",
            Category = "General",
            Icon = Icon.Info,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var choices = new (string Title, string Value)[]
        {
            ("Default (info)", ""),
            ("Verbose", "verbose"),
            ("Debug", "debug"),
            ("Warning", "warn"),
            ("Error only", "error"),
        };

        return FormElementHelper.CreateChoiceSet(
            nameof(state.Verbosity),
            "Log Level (--verbosity)",
            choices,
            state.Verbosity);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.Verbosity = FormElementHelper.GetString(formInput, nameof(state.Verbosity));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.Verbosity))
        {
            args.Add($"--verbosity={state.Verbosity}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.Verbosity = string.Empty;
    }
}