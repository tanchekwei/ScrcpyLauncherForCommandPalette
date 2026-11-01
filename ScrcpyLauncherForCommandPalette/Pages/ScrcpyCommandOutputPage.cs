// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Globalization;
using System.Text;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Pages;

public sealed partial class ScrcpyCommandOutputPage : ContentPage
{
    private const string MarkdownPrefix = "```\n";
    private const string MarkdownSuffix = "\n```";

    private readonly string _commandTitle;
    private readonly string _arguments;
    private readonly object _lock = new();

    private string? _output;
    private bool _isRunning;

    public ScrcpyCommandOutputPage(string commandTitle, string arguments, IconInfo? icon = null)
    {
        _commandTitle = commandTitle;
        _arguments = arguments;

        Title = commandTitle;
        Name = commandTitle;
        Icon = icon ?? Classes.Icon.Terminal;

        //StartExecution();
    }

    private void StartExecution()
    {
        lock (_lock)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _output = null;
        }

        try
        {
            var result = ScrcpyRunner.RunAndGetOutput(_arguments);
            lock (_lock)
            {
                _output = string.IsNullOrWhiteSpace(result) ? "(No output)" : result;
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            lock (_lock)
            {
                _output = $"Error running command: {ex.Message}";
            }
        }
        finally
        {
            lock (_lock)
            {
                _isRunning = false;
            }
        }
    }

    public override IContent[] GetContent()
    {
        try
        {
            lock (_lock)
            {
                StartExecution();
            }

            bool running;
            string? outText;            
            lock (_lock)
            {
                running = _isRunning;
                outText = _output;
            }

            var sb = new StringBuilder();
            sb.AppendLine(CultureInfo.InvariantCulture, $"# {_commandTitle}");
            sb.AppendLine();
            sb.AppendLine(CultureInfo.InvariantCulture, $"- **Command**: `scrcpy {_arguments}`");
            sb.AppendLine(CultureInfo.InvariantCulture, $"- **Status**: {(running ? "⏳ Running..." : "✅ Completed")}");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            if (running)
            {
                sb.AppendLine("Executing command, please wait...");
            }
            else if (string.IsNullOrWhiteSpace(outText))
            {
                sb.AppendLine("*(No output)*");
            }
            else
            {
                sb.Append(MarkdownPrefix);
                sb.Append(outText);
                sb.Append(MarkdownSuffix);
            }

            return [new MarkdownContent(sb.ToString())];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return [new MarkdownContent($"Error displaying output: {ex.Message}")];
        }
    }
}