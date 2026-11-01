// Modifications copyright (c) 2025 tanchekwei 
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ScrcpyLauncherForCommandPalette.Classes;

public static class Icon
{
    public static readonly IconInfo Extension = IconHelpers.FromRelativePath(@"Assets\icon.svg");
    public static readonly IconInfo FileExplorer = new IconInfo("\uEC50");
    public static readonly IconInfo Pinned = new IconInfo("\ue718");
    public static readonly IconInfo Unpinned = new IconInfo("\ue77a");
    public static readonly IconInfo Web = new("\uE774");
    public static readonly IconInfo Help = new IconInfo("\uE897");
    public static readonly IconInfo Info = new IconInfo("\uE946");
    public static readonly IconInfo Setting = new IconInfo("\uE713");
    public static readonly IconInfo Bug = new IconInfo("\uEBE8");
    public static readonly IconInfo EraseTool = new IconInfo("\uE75C");
    public static readonly IconInfo FilterIcon = new("\uE71C");

    // scrcpy specific icons
    public static readonly IconInfo Phone = new("\uE8EA");
    public static readonly IconInfo Video = new("\uE714");
    public static readonly IconInfo Camera = new("\uE720");
    public static readonly IconInfo Audio = new("\uE8D6");
    public static readonly IconInfo Cast = new("\uE777");
    public static readonly IconInfo Record = new("\uE722");
    public static readonly IconInfo Keyboard = new("\uE765");
    public static readonly IconInfo Mouse = new("\uE962");
    public static readonly IconInfo Gamepad = new("\uE7FC");
    public static readonly IconInfo Display = new("\uE7F4");
    public static readonly IconInfo Terminal = new("\uE756");
    public static readonly IconInfo Window = new("\uE737");
    public static readonly IconInfo Copy = new("\uE8A7");
    public static readonly IconInfo Admin = new("\uE7EF");
    public static readonly IconInfo Edit = new("\uE70F");
    public static readonly IconInfo Refresh = new("\uE72C");
    public static readonly IconInfo Flow = new("\uEF90");
    public static readonly IconInfo Cancel = new("\uE711");
    public static readonly IconInfo Check = new("\uE73E");
    public static readonly IconInfo Play = new("\uE768");
    public static readonly IconInfo Checkbox = new("\uE739");
    public static readonly IconInfo CheckboxComposite = new("\uE73A");
}
