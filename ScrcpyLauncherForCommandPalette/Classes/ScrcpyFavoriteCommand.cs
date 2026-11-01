// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;

namespace ScrcpyLauncherForCommandPalette.Classes;

public sealed class ScrcpyFavoriteCommand
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? TargetDeviceSerial { get; set; }
    public string? TargetDeviceModel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullCommand => string.IsNullOrWhiteSpace(Arguments) ? "scrcpy" : $"scrcpy {Arguments}";
}