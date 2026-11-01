// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using ScrcpyLauncherForCommandPalette.Features;

namespace ScrcpyLauncherForCommandPalette.Classes;

public static class ScrcpyCommands
{
    public static IReadOnlyList<ScrcpyCliCommand> All => ScrcpyFeatureRegistry.GetAllCommands();
}
