// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette.Features.ControlAndInput;

public sealed class NoClipboardAutosync : IScrcpyFeature
{
    public string Category => "Control & Input";

    public IReadOnlyList<ScrcpyCliCommand> Commands =>
    [
        new ScrcpyCliCommand
        {
            Id = "scrcpy.no-clipboard-autosync",
            Title = "Disable Clipboard Auto-Sync (--no-clipboard-autosync)",
            Arguments = "--no-clipboard-autosync",
            Description = "Disable automatic synchronization between device and PC clipboard",
            Category = "Control",
            Icon = Icon.Keyboard,
            DocumentationUrl = $"https://github.com/Genymobile/scrcpy/blob/{Constant.scrcpyVersion}/doc/control.md#copy-paste",
        },
    ];

    public JsonObject? CreateCardElement(ScrcpyCommandBuilderState state)
    {
        return FormElementHelper.CreateToggle(
            nameof(state.NoClipboardAutosync),
            "Disable Clipboard Auto-Sync (--no-clipboard-autosync)",
            state.NoClipboardAutosync);
    }

    public void UpdateState(ScrcpyCommandBuilderState state, JsonObject formInput)
    {
        state.NoClipboardAutosync = FormElementHelper.GetBool(formInput, nameof(state.NoClipboardAutosync));
    }

    public void BuildArguments(ScrcpyCommandBuilderState state, List<string> args)
    {
        if (state.NoClipboardAutosync)
        {
            args.Add("--no-clipboard-autosync");
        }
    }

    public void ResetState(ScrcpyCommandBuilderState state)
    {
        state.NoClipboardAutosync = false;
    }
}