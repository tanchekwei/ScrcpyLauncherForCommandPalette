// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Text.Json.Nodes;

namespace ScrcpyLauncherForCommandPalette.Features;

public static class FormElementHelper
{
    public static void AddNode(this JsonArray array, JsonNode? node)
    {
        ArgumentNullException.ThrowIfNull(array);
        ((System.Collections.Generic.IList<JsonNode?>)array).Add(node);
    }

    public static JsonObject CreateTextBlock(string text, string size = "Default", string weight = "Default", bool isSubtle = false, bool wrap = true, bool separator = false, string? style = null)
    {
        var tb = new JsonObject
        {
            ["type"] = "TextBlock",
            ["text"] = text,
            ["wrap"] = wrap,
        };
        if (size != "Default") tb["size"] = size;
        if (weight != "Default") tb["weight"] = weight;
        if (isSubtle) tb["isSubtle"] = true;
        if (separator) tb["separator"] = true;
        if (style != null) tb["style"] = style;
        return tb;
    }

    public static JsonObject CreateToggle(string id, string title, bool isChecked)
    {
        return new JsonObject
        {
            ["type"] = "Input.Toggle",
            ["id"] = id,
            ["title"] = title,
            ["value"] = isChecked ? "true" : "false",
            ["valueOn"] = "true",
            ["valueOff"] = "false",
        };
    }

    public static JsonObject CreateTextInput(string id, string label, string? value = "", string? placeholder = "", bool isMultiline = false)
    {
        var input = new JsonObject
        {
            ["type"] = "Input.Text",
            ["id"] = id,
            ["label"] = label,
            ["value"] = value ?? string.Empty,
        };
        if (!string.IsNullOrEmpty(placeholder)) input["placeholder"] = placeholder;
        if (isMultiline) input["isMultiline"] = true;
        return input;
    }

    public static JsonObject CreateChoiceSet(string id, string label, (string Title, string Value)[] choices, string selectedValue = "")
    {
        var choicesArray = new JsonArray();
        foreach (var (title, val) in choices)
        {
            choicesArray.AddNode(new JsonObject
            {
                ["title"] = title,
                ["value"] = val,
            });
        }

        return new JsonObject
        {
            ["type"] = "Input.ChoiceSet",
            ["id"] = id,
            ["label"] = label,
            ["style"] = "compact",
            ["value"] = selectedValue,
            ["choices"] = choicesArray,
        };
    }

    public static JsonObject CreateSubmitAction(string title, string actionName, string? style = null)
    {
        var act = new JsonObject
        {
            ["type"] = "Action.Submit",
            ["title"] = title,
            ["data"] = new JsonObject
            {
                ["action"] = actionName,
            },
        };
        if (style != null) act["style"] = style;
        return act;
    }

    public static bool GetBool(JsonObject obj, string key)
    {
        if (obj.TryGetPropertyValue(key, out var node) && node != null)
        {
            var str = node.ToString().Trim();
            return string.Equals(str, "true", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public static string GetString(JsonObject obj, string key)
    {
        if (obj.TryGetPropertyValue(key, out var node) && node != null)
        {
            return node.ToString().Trim();
        }
        return string.Empty;
    }
}