// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;

namespace ScrcpyLauncherForCommandPalette.Classes;

public sealed class AutoAdbCommand
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string ScrcpyArguments { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FormattedArguments
    {
        get
        {
            var clean = ScrcpyArguments?.Trim() ?? string.Empty;
            if (clean.Contains("{}"))
            {
                return clean;
            }

            if (clean.StartsWith("-s ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = clean.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    return $"-s {{}} {parts[2]}";
                }
                return "-s {}";
            }

            return string.IsNullOrEmpty(clean) ? "-s {}" : $"-s {{}} {clean}";
        }
    }

    public string FullCommand => $"autoadb scrcpy {FormattedArguments}".Trim();
}