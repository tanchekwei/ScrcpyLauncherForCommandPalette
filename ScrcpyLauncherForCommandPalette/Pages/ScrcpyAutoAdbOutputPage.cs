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

public sealed partial class ScrcpyAutoAdbOutputPage : ContentPage
{
    private const string MarkdownPrefix = "```\n";
    private const string MarkdownSuffix = "\n```";

    private readonly AutoAdbCommand _command;

    public ScrcpyAutoAdbOutputPage(AutoAdbCommand command)
    {
        try
        {
            _command = command;
            Title = $"Console Output";
            Name = $"Console Output";
            Icon = Classes.Icon.Terminal;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            _command = null!;
            throw;
        }
    }

    public override IContent[] GetContent()
    {
        try
        {
            bool isRunning = AutoAdbService.Instance.IsRunning(_command.Id);
            int? pid = isRunning ? AutoAdbService.Instance.GetProcessId(_command.Id) : null;
            string statusStr = isRunning ? $"🟢 Running (PID: {pid})" : "⚪ Stopped";

            var sb = new StringBuilder();
            sb.AppendLine(CultureInfo.InvariantCulture, $"# {_command.Name} - Console Output");
            sb.AppendLine();
            sb.AppendLine(CultureInfo.InvariantCulture, $"- **Status**: {statusStr}");
            sb.AppendLine(CultureInfo.InvariantCulture, $"- **Command**: `{_command.FullCommand}`");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            string logs = AutoAdbService.Instance.GetConsoleOutput(_command.Id);
            if (string.IsNullOrWhiteSpace(logs))
            {
                sb.AppendLine("```");
                sb.AppendLine("No console output recorded yet for this AutoAdb watcher.");
                sb.AppendLine("When an Android device connects, stdout and stderr output will appear here.");
                sb.AppendLine("```");
            }
            else
            {
                sb.Append(MarkdownPrefix);
                sb.Append(logs);
                sb.Append(MarkdownSuffix);
            }

            return [new MarkdownContent(sb.ToString())];
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return [new MarkdownContent($"Error retrieving console output: {ex.Message}")];
        }
    }
}