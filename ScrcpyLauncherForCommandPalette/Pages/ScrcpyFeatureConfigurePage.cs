// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Features;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyFeatureConfigurePage : ContentPage
{
    private readonly ScrcpyFeatureConfigureForm _form;

    public ScrcpyFeatureConfigurePage(
        IScrcpyFeature feature,
        string featureTitle,
        Action onUpdated,
        ScrcpyCommandBuilderService? service = null)
    {
        _form = new ScrcpyFeatureConfigureForm(feature, featureTitle, onUpdated, service);
        Name = $"Configure {featureTitle}";
        Title = $"Configure {featureTitle}";
        Icon = Classes.Icon.Setting;
    }

    public override IContent[] GetContent()
    {
        return [_form];
    }
}

internal sealed partial class ScrcpyFeatureConfigureForm : FormContent
{
    private readonly IScrcpyFeature _feature;
    private readonly string _featureTitle;
    private readonly Action _onUpdated;
    private readonly ScrcpyCommandBuilderService _service;

    public ScrcpyFeatureConfigureForm(
        IScrcpyFeature feature,
        string featureTitle,
        Action onUpdated,
        ScrcpyCommandBuilderService? service = null)
    {
        _feature = feature;
        _featureTitle = featureTitle;
        _onUpdated = onUpdated;
        _service = service ?? ScrcpyCommandBuilderService.Instance;

        var builderService = _service;
        UpdateCard();
    }

    private void UpdateCard()
    {
        var builderService = _service;
        var element = _feature.CreateCardElement(builderService.State);

        var body = new JsonArray();
        body.AddNode(FormElementHelper.CreateTextBlock($"Configure {_featureTitle}", size: "Large", weight: "Bolder"));
        body.AddNode(FormElementHelper.CreateTextBlock($"Category: {_feature.Category}", isSubtle: true));

        if (element != null)
        {
            body.AddNode(element);
        }
        else
        {
            body.AddNode(FormElementHelper.CreateTextBlock("No additional configuration required for this feature.", isSubtle: true));
        }

        var actions = new JsonArray();
        actions.AddNode(FormElementHelper.CreateSubmitAction("Save & Apply", "save", "positive"));
        actions.AddNode(FormElementHelper.CreateSubmitAction("Clear Feature Value", "clear", "destructive"));

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
            ["body"] = body,
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

            if (action == "clear")
            {
                _feature.ResetState(service.State);
                service.RebuildFinalCommand();
                _onUpdated.Invoke();
                new ToastStatusMessage($"Cleared {_featureTitle}").Show();
                return CommandResult.GoBack();
            }

            JsonObject? formInput = null;
            if (!string.IsNullOrEmpty(inputs))
            {
                formInput = JsonNode.Parse(inputs)?.AsObject();
            }

            if (formInput != null)
            {
                _feature.UpdateState(service.State, formInput);
                service.RebuildFinalCommand();
                _onUpdated.Invoke();
                new ToastStatusMessage($"Updated {_featureTitle}").Show();
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