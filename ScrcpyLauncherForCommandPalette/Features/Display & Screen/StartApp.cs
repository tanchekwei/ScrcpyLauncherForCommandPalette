// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Pages;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class StartApp : IScrcpyFeature, IScrcpyCustomPageFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.start-app",
            Title = "Start App",
            Arguments = "--start-app",
            Description = "Start an Android application on launch (--start-app)",
            Category = "Display & Screen",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#start-android-app",
        }
    ];

    public IPage CreateCustomPage(ScrcpyCommandBuilderService service, Action onUpdated)
    {
        return new StartAppListPage(this, service, onUpdated);
    }

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        var container = new JsonObject
        {
            ["type"] = "Container",
            ["title"] = "Start App (--start-app)",
            ["items"] = new JsonArray(),
        };
        var items = (JsonArray)container["items"]!;

        items.AddNode(FormElementHelper.CreateTextInput(
            nameof(state.StartApp),
            "Start App Package or Query (--start-app=)",
            state.StartApp,
            "e.g. org.mozilla.firefox, or ?firefox, or +org.mozilla.firefox"));

        return container;
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        string manualApp = FormElementHelper.GetString(formInput, nameof(state.StartApp));
        if (string.IsNullOrEmpty(manualApp))
        {
            manualApp = FormElementHelper.GetString(formInput, "SelectedApp");
        }
        state.StartApp = manualApp.Trim();
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (!string.IsNullOrWhiteSpace(state.StartApp))
        {
            args.Add($"--start-app={state.StartApp.Trim()}");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.StartApp = string.Empty;
    }
}