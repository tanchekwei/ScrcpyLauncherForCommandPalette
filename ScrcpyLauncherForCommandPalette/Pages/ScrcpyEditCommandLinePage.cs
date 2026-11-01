// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Features;
using ScrcpyLauncherForCommandPalette.Helpers;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyEditCommandLinePage : ContentPage
{
    private readonly ScrcpyEditCommandLineForm _form;

    public ScrcpyEditCommandLinePage(
        string commandLine,
        DeviceInfo? targetDevice,
        Action onUpdated,
        ScrcpyCommandBuilderService? service = null)
    {
        _form = new ScrcpyEditCommandLineForm(commandLine, targetDevice, onUpdated, service);
        Name = "Edit Command Line";
        Title = "Edit Command Line";
        Icon = Classes.Icon.Edit;
    }

    public override IContent[] GetContent()
    {
        return [_form];
    }
}

internal sealed partial class ScrcpyEditCommandLineForm : FormContent
{
    private readonly string _commandLine;
    private readonly DeviceInfo? _targetDevice;
    private readonly Action _onUpdated;
    private readonly ScrcpyCommandBuilderService _service;

    public ScrcpyEditCommandLineForm(
        string commandLine,
        DeviceInfo? targetDevice,
        Action onUpdated,
        ScrcpyCommandBuilderService? service = null)
    {
        _commandLine = commandLine;
        _targetDevice = targetDevice;
        _onUpdated = onUpdated;
        _service = service ?? ScrcpyCommandBuilderService.Instance;

        var body = new JsonArray();
        body.AddNode(FormElementHelper.CreateTextBlock(
            "Edit Command Line",
            size: "Large",
            weight: "Bolder"));
        body.AddNode(FormElementHelper.CreateTextBlock(
            "Update the scrcpy command line or custom arguments below.",
            isSubtle: true));

        if (_targetDevice != null)
        {
            body.AddNode(FormElementHelper.CreateTextBlock(
                $"Target Device: {_targetDevice.Model} ({_targetDevice.Serial})",
                isSubtle: true));
        }

        body.AddNode(FormElementHelper.CreateTextInput(
            "command_line",
            "Command Line",
            value: _commandLine,
            placeholder: "e.g. scrcpy -s <serial> --always-on-top",
            isMultiline: true));

        var actions = new JsonArray();
        actions.AddNode(FormElementHelper.CreateSubmitAction("Save & Apply", "save", "positive"));
        actions.AddNode(FormElementHelper.CreateSubmitAction("Reset to Default", "reset", "destructive"));

        body.AddNode(new JsonObject
        {
            ["type"] = "ActionSet",
            ["actions"] = actions
        });

        var card = new JsonObject
        {
            ["$schema"] = "http://adaptivecards.io/schemas/adaptive-card.json",
            ["type"] = "AdaptiveCard",
            ["version"] = "1.5",
            ["body"] = body
        };

        TemplateJson = card.ToJsonString();
    }

    public override ICommandResult SubmitForm(string inputs) => SubmitForm(inputs, string.Empty);

    public override ICommandResult SubmitForm(string inputs, string data)
    {
        try
        {
            var service = _service;
            string action = "save";
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var dataObj = JsonNode.Parse(data)?.AsObject();
                    var act = dataObj?["action"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(act))
                    {
                        action = act.Trim().ToLowerInvariant();
                    }
                }
                catch
                {
                }
            }

            if (action == "reset")
            {
                service.Reset(_targetDevice);
                _onUpdated.Invoke();
                new ToastStatusMessage("Reset options to default.").Show();
                return CommandResult.GoBack();
            }

            JsonObject? formInput = null;
            if (!string.IsNullOrEmpty(inputs))
            {
                formInput = JsonNode.Parse(inputs)?.AsObject();
            }

            if (formInput != null)
            {
                string updatedCommandLine = FormElementHelper.GetString(formInput, "command_line");
                if (string.IsNullOrWhiteSpace(updatedCommandLine))
                {
                    updatedCommandLine = "scrcpy";
                }

                // Reset feature states so removed flags are cleared
                foreach (var feature in ScrcpyFeatureRegistry.Features)
                {
                    feature.ResetState(service.State);
                }

                service.State.LoadFromArguments(updatedCommandLine);

                // Preserve target device serial if none was specified in the arguments
                if (_targetDevice != null && string.IsNullOrEmpty(service.State.DeviceSerial) &&
                    !updatedCommandLine.Contains("-s") && !updatedCommandLine.Contains("--serial"))
                {
                    service.State.DeviceSerial = _targetDevice.Serial;
                }

                service.RebuildFinalCommand();

                if (service.State.FinalCommand == "scrcpy" &&
                    !string.IsNullOrWhiteSpace(updatedCommandLine) &&
                    updatedCommandLine.Trim() != "scrcpy")
                {
                    service.State.FinalCommand = updatedCommandLine.Trim();
                }

                _onUpdated.Invoke();
                new ToastStatusMessage("Updated command line.").Show();
            }

            return CommandResult.GoBack();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            new ToastStatusMessage($"Error saving: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }
}