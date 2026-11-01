// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.DisplayAndScreen;

public sealed class KeepActive : IScrcpyFeature
{
    public string Category => "Display & Screen";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.keep-active",
            Title = "Keep Active",
            Arguments = "--keep-active",
            Description = "Periodically signals user activity to prevent device from sleeping",
            Category = "Device",
            Icon = Icon.Phone,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/device.md#keep-active",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.KeepActive),
            "Keep Active (Signal Activity) (--keep-active)",
            state.KeepActive);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.KeepActive = FormElementHelper.GetBool(formInput, nameof(state.KeepActive));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.KeepActive)
        {
            args.Add("--keep-active");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.KeepActive = false;
    }
}