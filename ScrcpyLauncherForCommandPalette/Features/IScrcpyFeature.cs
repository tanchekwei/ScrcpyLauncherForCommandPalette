// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Features;

public interface IScrcpyFeature
{
    string Category { get; }
    IReadOnlyList<ScrcpyCliCommand> Commands { get; }
    JsonObject? CreateCardElement(ScrcpyCommandBuilderState state);
    void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput);
    void BuildArguments(ScrcpyCommandBuilderState state, List<string> args);
    void ResetState(ScrcpyCommandBuilderState state);
}

public interface IScrcpyCustomPageFeature
{
    IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated);
}