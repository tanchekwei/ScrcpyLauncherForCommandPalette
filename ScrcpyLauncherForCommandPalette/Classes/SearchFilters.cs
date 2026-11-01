// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;

namespace ScrcpyLauncherForCommandPalette;

public sealed partial class SearchFilters : Filters
{
    public const string FilterAll = "All";
    public const string FilterGeneral = "General";
    public const string FilterDeviceAndConnection = "Device & Connection";
    public const string FilterDisplayAndScreen = "Display & Screen";
    public const string FilterWindowOptions = "Window Options";
    public const string FilterVideoAndPerformance = "Video & Performance";
    public const string FilterAudio = "Audio";
    public const string FilterCameraMode = "Camera Mode";
    public const string FilterControlAndInput = "Control & Input";
    public const string FilterRecording = "Recording";
    public const string FilterVirtualDisplay = "Virtual Display";
    public const string FilterCustomArguments = "Custom Arguments";
    public const string FilterDiagnostic = "Diagnostic";

    public SearchFilters()
    {
        CurrentFilterId = FilterAll;
    }

    public override IFilterItem[] GetFilters()
    {
        return [
            new Filter { Id = FilterAll, Name = "All", Icon = Icon.FilterIcon },
            new Separator(),
            new Filter { Id = FilterDeviceAndConnection, Name = "Device & Connection", Icon = Icon.Cast },
            new Filter { Id = FilterDisplayAndScreen, Name = "Display & Screen", Icon = Icon.Display },
            new Filter { Id = FilterWindowOptions, Name = "Window Options", Icon = Icon.Window },
            new Filter { Id = FilterVideoAndPerformance, Name = "Video & Performance", Icon = Icon.Video },
            new Filter { Id = FilterAudio, Name = "Audio", Icon = Icon.Audio },
            new Filter { Id = FilterCameraMode, Name = "Camera Mode", Icon = Icon.Camera },
            new Filter { Id = FilterControlAndInput, Name = "Control & Input", Icon = Icon.Keyboard },
            new Filter { Id = FilterRecording, Name = "Recording", Icon = Icon.Record },
            new Filter { Id = FilterVirtualDisplay, Name = "Virtual Display", Icon = Icon.Display },
            new Filter { Id = FilterCustomArguments, Name = "Custom Arguments", Icon = Icon.Terminal },
            new Filter { Id = FilterDiagnostic, Name = "Diagnostic", Icon = Icon.Bug },
            new Filter { Id = FilterGeneral, Name = "General", Icon = Icon.Phone },
        ];
    }
}