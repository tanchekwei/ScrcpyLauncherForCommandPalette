// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ScrcpyLauncherForCommandPalette.Classes;

public sealed class ScrcpyCliCommand
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Arguments { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public IconInfo Icon { get; init; } = Classes.Icon.Extension;
    public bool RequiresConsole { get; init; }
    public string? DocumentationUrl { get; init; }

    public string FullCommand => string.IsNullOrWhiteSpace(Arguments) ? "scrcpy" : $"scrcpy {Arguments}";
}