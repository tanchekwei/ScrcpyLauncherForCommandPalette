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

public sealed partial class ScrcpySaveFavoritePage : ContentPage
{
    private readonly ScrcpySaveFavoriteForm _form;

    public ScrcpySaveFavoritePage(
        string commandLine,
        DeviceInfo? targetDevice,
        ScrcpyFavoriteCommand? existingFavorite,
        SettingsManager? settingsManager,
        Action onSaved,
        ScrcpyCommandBuilderService? service = null)
    {
        _form = new ScrcpySaveFavoriteForm(commandLine, targetDevice, existingFavorite, settingsManager, onSaved, service);

        if (existingFavorite != null)
        {
            Name = $"Update Preset: {existingFavorite.Name}";
            Title = $"Update Preset: {existingFavorite.Name}";
            Icon = Classes.Icon.Edit;
        }
        else
        {
            Name = "Save to Favorites";
            Title = "Save Configuration to Favorites";
            Icon = Classes.Icon.Pinned;
        }
    }

    public override IContent[] GetContent()
    {
        return [_form];
    }
}

internal sealed partial class ScrcpySaveFavoriteForm : FormContent
{
    private readonly string _commandLine;
    private readonly DeviceInfo? _targetDevice;
    private readonly ScrcpyFavoriteCommand? _existingFavorite;
    private readonly SettingsManager? _settingsManager;
    private readonly Action _onSaved;
    private readonly ScrcpyCommandBuilderService _service;
    private readonly string _defaultName;

    public ScrcpySaveFavoriteForm(
        string commandLine,
        DeviceInfo? targetDevice,
        ScrcpyFavoriteCommand? existingFavorite,
        SettingsManager? settingsManager,
        Action onSaved,
        ScrcpyCommandBuilderService? service = null)
    {
        _commandLine = commandLine;
        _targetDevice = targetDevice;
        _existingFavorite = existingFavorite;
        _settingsManager = settingsManager;
        _onSaved = onSaved;
        _service = service ?? ScrcpyCommandBuilderService.Instance;

        var state = _service.State;
        _defaultName = !string.IsNullOrWhiteSpace(_existingFavorite?.Name)
            ? _existingFavorite.Name
            : (!string.IsNullOrWhiteSpace(state.FavoriteName)
                ? state.FavoriteName.Trim()
                : $"Preset ({DateTime.Now:MMM dd, HH:mm})");

        var body = new JsonArray();
        body.AddNode(FormElementHelper.CreateTextBlock(
            _existingFavorite != null ? "Update Preset" : "Save to Favorites",
            size: "Large",
            weight: "Bolder"));
        body.AddNode(FormElementHelper.CreateTextBlock($"Command: {_commandLine}", isSubtle: true));

        if (_targetDevice != null)
        {
            body.AddNode(FormElementHelper.CreateTextBlock(
                $"Device: {_targetDevice.Model} ({_targetDevice.Serial})",
                isSubtle: true));
        }

        body.AddNode(FormElementHelper.CreateTextInput(
            "preset_name",
            "Preset Name",
            placeholder: $"Enter preset name (default: {_defaultName})"));

        var actions = new JsonArray();
        actions.AddNode(FormElementHelper.CreateSubmitAction("Save", "save", "positive"));
        actions.AddNode(FormElementHelper.CreateSubmitAction("Use Default Name", "default"));

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

            string finalName;
            if (action == "default")
            {
                finalName = _defaultName;
            }
            else
            {
                JsonObject? formInput = null;
                if (!string.IsNullOrEmpty(inputs))
                {
                    formInput = JsonNode.Parse(inputs)?.AsObject();
                }

                var enteredName = formInput?["preset_name"]?.ToString()?.Trim();
                finalName = !string.IsNullOrWhiteSpace(enteredName) ? enteredName : _defaultName;
            }

            var state = _service.State;
            string args = ScrcpyCommandBuilderState.ExtractArguments(_commandLine);

            var fav = new ScrcpyFavoriteCommand
            {
                Id = _existingFavorite?.Id ?? Guid.NewGuid().ToString("N"),
                Name = finalName,
                Arguments = args,
                Description = _commandLine,
                TargetDeviceSerial = _targetDevice?.Serial ?? state.DeviceSerial,
                TargetDeviceModel = _targetDevice?.Model ?? _existingFavorite?.TargetDeviceModel,
                CreatedAt = _existingFavorite?.CreatedAt ?? DateTime.UtcNow,
            };

            var sm = SettingsManager.Instance ?? ScrcpyRunner.SettingsManager ?? _settingsManager;
            if (sm != null)
            {
                sm.AddFavorite(fav);
                state.FavoriteName = finalName;
                if (_existingFavorite != null)
                {
                    _existingFavorite.Name = finalName;
                    _existingFavorite.Arguments = args;
                    _existingFavorite.Description = _commandLine;
                }

                string msg = _existingFavorite != null ? $"Updated preset '{finalName}'!" : $"Saved '{finalName}' to Favorites!";
                new ToastStatusMessage(msg).Show();
            }
            else
            {
                new ToastStatusMessage("Settings not initialized.").Show();
            }

            _onSaved.Invoke();
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