// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Linq;

namespace ScrcpyLauncherForCommandPalette.Features;

public static class ScrcpyFeatureRegistry
{
    public static readonly IReadOnlyList<IScrcpyFeature> Features =
    [
        // General
        new General.PauseOnExit(),
        new General.Verbosity(),

        // Device & Connection
        new DeviceAndConnection.DeviceSerial(),
        new DeviceAndConnection.TcpIp(),
        new DeviceAndConnection.SelectUsb(),
        new DeviceAndConnection.SelectTcpIp(),
        new DeviceAndConnection.ForceAdbForward(),
        new DeviceAndConnection.NoCleanup(),
        new DeviceAndConnection.PushTarget(),
        new DeviceAndConnection.PortRange(),
        new DeviceAndConnection.TunnelHost(),
        new DeviceAndConnection.TunnelPort(),
        new DeviceAndConnection.KillAdbOnClose(),

        // Display & Screen
        new DisplayAndScreen.TurnScreenOff(),
        new DisplayAndScreen.StayAwake(),
        new DisplayAndScreen.KeepActive(),
        new DisplayAndScreen.ShowTouches(),
        new DisplayAndScreen.PowerOffOnClose(),
        new DisplayAndScreen.NoPowerOn(),
        new DisplayAndScreen.ScreenOffTimeout(),
        new DisplayAndScreen.StartApp(),

        // Window Options
        new WindowOptions.Fullscreen(),
        new WindowOptions.AlwaysOnTop(),
        new WindowOptions.WindowBorderless(),
        new WindowOptions.DisableScreensaver(),
        new WindowOptions.WindowTitle(),
        new WindowOptions.WindowX(),
        new WindowOptions.WindowY(),
        new WindowOptions.WindowWidth(),
        new WindowOptions.WindowHeight(),
        new WindowOptions.NoWindowAspectRatioLock(),
        new WindowOptions.BackgroundColor(),
        new WindowOptions.RenderFit(),
        new WindowOptions.TimeLimit(),
        new WindowOptions.RenderDriver(),
        new WindowOptions.NoTerminalTitle(),
        new WindowOptions.NoWindow(),

        // Video & Performance
        new VideoAndPerformance.NoVideo(),
        new VideoAndPerformance.MaxSize(),
        new VideoAndPerformance.VideoBitRate(),
        new VideoAndPerformance.MaxFps(),
        new VideoAndPerformance.VideoCodec(),
        new VideoAndPerformance.VideoEncoder(),
        new VideoAndPerformance.VideoCodecOptions(),
        new VideoAndPerformance.Hwdec(),
        new VideoAndPerformance.CaptureOrientation(),
        new VideoAndPerformance.DisplayOrientation(),
        new VideoAndPerformance.IgnoreVideoEncoderConstraints(),
        new VideoAndPerformance.VideoBuffer(),
        new VideoAndPerformance.PrintFps(),
        new VideoAndPerformance.DisplayId(),
        new VideoAndPerformance.VideoOrientation(),
        new VideoAndPerformance.Angle(),
        new VideoAndPerformance.Crop(),
        new VideoAndPerformance.NoVideoPlayback(),
        new VideoAndPerformance.NoDownsizeOnError(),
        new VideoAndPerformance.NoMipmaps(),
        new VideoAndPerformance.MinSizeAlignment(),

        // Audio
        new Audio.NoAudio(),
        new Audio.NoAudioPlayback(),
        new Audio.AudioSource(),
        new Audio.AudioDup(),
        new Audio.AudioCodec(),
        new Audio.AudioEncoder(),
        new Audio.AudioCodecOptions(),
        new Audio.AudioBitRate(),
        new Audio.AudioBuffer(),
        new Audio.AudioOutputBuffer(),
        new Audio.RequireAudio(),

        // Camera Mode
        new CameraMode.CameraSource(),
        new CameraMode.CameraFacing(),
        new CameraMode.CameraId(),
        new CameraMode.CameraSize(),
        new CameraMode.CameraAr(),
        new CameraMode.CameraFps(),
        new CameraMode.CameraZoom(),
        new CameraMode.CameraTorch(),
        new CameraMode.CameraHighSpeed(),

        // Control & Input
        new ControlAndInput.NoControl(),
        new ControlAndInput.OtgMode(),
        new ControlAndInput.KeyboardMode(),
        new ControlAndInput.MouseMode(),
        new ControlAndInput.GamepadMode(),
        new ControlAndInput.PreferText(),
        new ControlAndInput.RawKeyEvents(),
        new ControlAndInput.NoKeyRepeat(),
        new ControlAndInput.MouseBind(),
        new ControlAndInput.NoMouseHover(),
        new ControlAndInput.ShortcutMod(),
        new ControlAndInput.LegacyPaste(),
        new ControlAndInput.NoClipboardAutosync(),

        // Recording
        new Recording.RecordFile(),
        new Recording.RecordFormat(),
        new Recording.RecordOrientation(),
        new Recording.NoPlayback(),

        // Virtual Display
        new VirtualDisplay.NewDisplay(),
        new VirtualDisplay.FlexDisplay(),
        new VirtualDisplay.DisplayImePolicy(),
        new VirtualDisplay.NoVdSystemDecorations(),
        new VirtualDisplay.NoVdDestroyContent(),

        // Custom Arguments
        new CustomArguments.ExtraArgs(),

        // Diagnostic
        new Diagnostic.ListEncoders(),
        new Diagnostic.ListDisplays(),
        new Diagnostic.ListApps(),
        new Diagnostic.ListCameras(),
        new Diagnostic.ListCameraSizes(),
        new Diagnostic.Version(),
        new Diagnostic.Help(),
    ];

    public static IReadOnlyList<Classes.ScrcpyCliCommand> GetAllCommands()
    {
        return [.. Features.SelectMany(f => f.Commands)];
    }
}