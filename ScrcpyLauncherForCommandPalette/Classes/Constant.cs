// Copyright (c) 2026 tanchekwei 
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ScrcpyLauncherForCommandPalette.Classes;

public static class Constant
{
#if DEBUG
    public const string AppName = "ScrcpyLauncherForCommandPaletteDev";
    public const string ScrcpyLauncherDisplayName = "scrcpy Launcher (Dev)";
#else
    public const string AppName = "ScrcpyLauncherForCommandPalette";
    public const string ScrcpyLauncherDisplayName = "scrcpy Launcher";
#endif
    public static readonly string AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    public static readonly string SettingsFolderPath = Utilities.BaseSettingsPath(AppName);
    public const string scrcpyVersion = "v5.0.1";
}