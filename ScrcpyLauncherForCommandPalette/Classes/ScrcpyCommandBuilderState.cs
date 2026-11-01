// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using ScrcpyLauncherForCommandPalette.Features;
using ScrcpyLauncherForCommandPalette.Features.Diagnostic;
using ScrcpyLauncherForCommandPalette.Helpers;

namespace ScrcpyLauncherForCommandPalette.Classes;

public sealed class ScrcpyCommandBuilderState
{
    public string DeviceSerial { get; set; } = string.Empty;
    public string FavoriteName { get; set; } = string.Empty;
    public string TcpIp { get; set; } = string.Empty;
    public bool SelectUsb { get; set; }
    public bool SelectTcpIp { get; set; }
    public bool ForceAdbForward { get; set; }
    public bool NoCleanup { get; set; }
    public string PushTarget { get; set; } = string.Empty;
    public string PortRange { get; set; } = string.Empty;
    public string TunnelHost { get; set; } = string.Empty;
    public string TunnelPort { get; set; } = string.Empty;
    public bool KillAdbOnClose { get; set; }

    public bool StayAwake { get; set; }
    public bool TurnScreenOff { get; set; }
    public bool ShowTouches { get; set; }
    public bool PowerOffOnClose { get; set; }
    public bool NoPowerOn { get; set; }
    public bool KeepActive { get; set; }
    public string ScreenOffTimeout { get; set; } = string.Empty;
    public string StartApp { get; set; } = string.Empty;
    public List<AppInfo> InstalledApps { get; set; } = [];
    public bool IsLoadingApps { get; set; }

    public bool Fullscreen { get; set; }
    public bool AlwaysOnTop { get; set; }
    public bool Borderless { get; set; }
    public bool DisableScreensaver { get; set; }
    public string WindowTitle { get; set; } = string.Empty;
    public string WindowX { get; set; } = string.Empty;
    public string WindowY { get; set; } = string.Empty;
    public string WindowWidth { get; set; } = string.Empty;
    public string WindowHeight { get; set; } = string.Empty;
    public bool NoWindowAspectRatioLock { get; set; }
    public string BackgroundColor { get; set; } = string.Empty;
    public string RenderFit { get; set; } = string.Empty;
    public bool NoWindow { get; set; }
    public string TimeLimit { get; set; } = string.Empty;
    public string RenderDriver { get; set; } = string.Empty;
    public bool NoTerminalTitle { get; set; }

    public bool NoVideo { get; set; }
    public string MaxSize { get; set; } = string.Empty;
    public string MaxFps { get; set; } = string.Empty;
    public string VideoBitRate { get; set; } = string.Empty;
    public string VideoCodec { get; set; } = string.Empty;
    public string VideoEncoder { get; set; } = string.Empty;
    public string VideoCodecOptions { get; set; } = string.Empty;
    public string Hwdec { get; set; } = string.Empty;
    public string CaptureOrientation { get; set; } = string.Empty;
    public string DisplayOrientation { get; set; } = string.Empty;
    public bool IgnoreVideoEncoderConstraints { get; set; }
    public string VideoBuffer { get; set; } = string.Empty;
    public bool PrintFps { get; set; }
    public string DisplayId { get; set; } = string.Empty;
    public string VideoOrientation { get; set; } = string.Empty;
    public string Angle { get; set; } = string.Empty;
    public string Crop { get; set; } = string.Empty;
    public bool NoVideoPlayback { get; set; }
    public bool NoDownsizeOnError { get; set; }
    public bool NoMipmaps { get; set; }
    public string MinSizeAlignment { get; set; } = string.Empty;

    public bool NoAudio { get; set; }
    public bool NoAudioPlayback { get; set; }
    public bool AudioDup { get; set; }
    public string AudioSource { get; set; } = string.Empty;
    public string AudioCodec { get; set; } = string.Empty;
    public string AudioEncoder { get; set; } = string.Empty;
    public string AudioCodecOptions { get; set; } = string.Empty;
    public string AudioBuffer { get; set; } = string.Empty;
    public string AudioOutputBuffer { get; set; } = string.Empty;
    public string AudioBitRate { get; set; } = string.Empty;
    public bool RequireAudio { get; set; }

    public bool CameraSource { get; set; }
    public string CameraFacing { get; set; } = string.Empty;
    public string CameraId { get; set; } = string.Empty;
    public string CameraSize { get; set; } = string.Empty;
    public string CameraFps { get; set; } = string.Empty;
    public bool CameraTorch { get; set; }
    public bool CameraHighSpeed { get; set; }
    public string CameraAr { get; set; } = string.Empty;
    public string CameraZoom { get; set; } = string.Empty;

    public bool NoControl { get; set; }
    public bool OtgMode { get; set; }
    public string KeyboardMode { get; set; } = string.Empty;
    public string MouseMode { get; set; } = string.Empty;
    public string GamepadMode { get; set; } = string.Empty;
    public bool PreferText { get; set; }
    public bool RawKeyEvents { get; set; }
    public bool NoKeyRepeat { get; set; }
    public string MouseBind { get; set; } = string.Empty;
    public bool NoMouseHover { get; set; }
    public string ShortcutMod { get; set; } = string.Empty;
    public bool LegacyPaste { get; set; }
    public bool NoClipboardAutosync { get; set; }

    public string RecordFile { get; set; } = string.Empty;
    public string RecordFormat { get; set; } = string.Empty;
    public string RecordOrientation { get; set; } = string.Empty;
    public bool RecordAudioOnly { get; set; }
    public bool NoPlayback { get; set; }

    public string NewDisplay { get; set; } = string.Empty;
    public bool FlexDisplay { get; set; }
    public string DisplayImePolicy { get; set; } = string.Empty;
    public bool NoVdSystemDecorations { get; set; }
    public bool NoVdDestroyContent { get; set; }

    public string PauseOnExit { get; set; } = string.Empty;
    public string Verbosity { get; set; } = string.Empty;

    public string ExtraArgs { get; set; } = string.Empty;
    public string FinalCommand { get; set; } = "scrcpy";

    public void Reset(DeviceInfo? targetDevice)
    {
        FavoriteName = string.Empty;

        foreach (var feature in ScrcpyFeatureRegistry.Features)
        {
            feature.ResetState(this);
        }

        var newSerial = targetDevice?.Serial ?? string.Empty;
        if (!string.Equals(newSerial, DeviceSerial, StringComparison.OrdinalIgnoreCase))
        {
            InstalledApps = [];
            IsLoadingApps = false;
        }

        DeviceSerial = newSerial;
        FinalCommand = !string.IsNullOrEmpty(DeviceSerial) ? $"scrcpy -s {DeviceSerial}" : "scrcpy";
    }

    public void UpdateFrom(JsonObject formInput)
    {
        if (formInput.TryGetPropertyValue(nameof(FinalCommand), out var fc) && fc != null)
        {
            FinalCommand = fc.ToString();
        }

        FavoriteName = FormElementHelper.GetString(formInput, nameof(FavoriteName));

        foreach (var feature in ScrcpyFeatureRegistry.Features)
        {
            feature.UpdateState(this, formInput);
        }
    }

    public static string BuildFullCommand(ScrcpyCommandBuilderState state)
    {
        var args = BuildArguments(state);
        return string.IsNullOrEmpty(args) ? "scrcpy" : $"scrcpy {args}";
    }

    public static string BuildArguments(ScrcpyCommandBuilderState state)
    {
        var args = new List<string>();

        foreach (var feature in ScrcpyFeatureRegistry.Features)
        {
            feature.BuildArguments(state, args);
        }

        return string.Join(" ", args);
    }

    public static string ExtractArguments(string commandLine)
    {
        var trimmed = commandLine.Trim();
        if (trimmed.StartsWith("scrcpy", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.Length > 6 ? trimmed[6..].Trim() : string.Empty;
        }
        return trimmed;
    }

    public void LoadFromArguments(string rawArguments)
    {
        var tokens = ParseTokens(rawArguments);
        var unhandled = new List<string>();

        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];
            if (token.Equals("scrcpy", StringComparison.OrdinalIgnoreCase)) continue;

            // Handle specific case-sensitive flags
            if (token == "-S")
            {
                TurnScreenOff = true;
                continue;
            }

            int eqIdx = token.IndexOf('=');
            string key = eqIdx >= 0 ? token[..eqIdx] : token;
            string? val = eqIdx >= 0 ? token[(eqIdx + 1)..] : null;

            string GetNextValue()
            {
                if (val != null) return val;
                if (i + 1 < tokens.Count && !tokens[i + 1].StartsWith('-'))
                {
                    i++;
                    return tokens[i];
                }
                return string.Empty;
            }

            switch (key.ToLowerInvariant())
            {
                case "-s":
                case "--serial":
                    DeviceSerial = GetNextValue();
                    break;
                case "--start-app":
                    StartApp = GetNextValue();
                    break;
                case "--screen-off-timeout":
                    ScreenOffTimeout = GetNextValue();
                    break;
                case "--window-title":
                    WindowTitle = GetNextValue();
                    break;
                case "--window-x":
                    WindowX = GetNextValue();
                    break;
                case "--window-y":
                    WindowY = GetNextValue();
                    break;
                case "--window-width":
                    WindowWidth = GetNextValue();
                    break;
                case "--window-height":
                    WindowHeight = GetNextValue();
                    break;
                case "--background-color":
                    BackgroundColor = GetNextValue();
                    break;
                case "--render-fit":
                    RenderFit = GetNextValue();
                    break;
                case "--time-limit":
                    TimeLimit = GetNextValue();
                    break;
                case "--render-driver":
                    RenderDriver = GetNextValue();
                    break;
                case "-m":
                case "--max-size":
                    MaxSize = GetNextValue();
                    break;
                case "--max-fps":
                    MaxFps = GetNextValue();
                    break;
                case "-b":
                case "--video-bit-rate":
                    VideoBitRate = GetNextValue();
                    break;
                case "--video-codec":
                    VideoCodec = GetNextValue();
                    break;
                case "--video-encoder":
                    VideoEncoder = GetNextValue();
                    break;
                case "--video-codec-options":
                    VideoCodecOptions = GetNextValue();
                    break;
                case "--hwdec":
                    var hwVal = GetNextValue();
                    Hwdec = string.IsNullOrWhiteSpace(hwVal) ? "auto" : hwVal;
                    break;
                case "--capture-orientation":
                    CaptureOrientation = GetNextValue();
                    break;
                case "--display-orientation":
                    DisplayOrientation = GetNextValue();
                    break;
                case "--video-buffer":
                    VideoBuffer = GetNextValue();
                    break;
                case "--display-id":
                    DisplayId = GetNextValue();
                    break;
                case "--orientation":
                    VideoOrientation = GetNextValue();
                    break;
                case "--angle":
                    Angle = GetNextValue();
                    break;
                case "--crop":
                    Crop = GetNextValue();
                    break;
                case "--min-size-alignment":
                    MinSizeAlignment = GetNextValue();
                    break;
                case "--audio-source":
                    AudioSource = GetNextValue();
                    break;
                case "--audio-codec":
                    AudioCodec = GetNextValue();
                    break;
                case "--audio-encoder":
                    AudioEncoder = GetNextValue();
                    break;
                case "--audio-codec-options":
                    AudioCodecOptions = GetNextValue();
                    break;
                case "--audio-buffer":
                    AudioBuffer = GetNextValue();
                    break;
                case "--audio-output-buffer":
                    AudioOutputBuffer = GetNextValue();
                    break;
                case "--audio-bit-rate":
                    AudioBitRate = GetNextValue();
                    break;
                case "--camera-facing":
                    CameraFacing = GetNextValue();
                    break;
                case "--camera-id":
                    CameraId = GetNextValue();
                    break;
                case "--camera-size":
                    CameraSize = GetNextValue();
                    break;
                case "--camera-ar":
                    CameraAr = GetNextValue();
                    break;
                case "--camera-fps":
                    CameraFps = GetNextValue();
                    break;
                case "--camera-zoom":
                    CameraZoom = GetNextValue();
                    break;
                case "--keyboard":
                    KeyboardMode = GetNextValue();
                    break;
                case "--mouse":
                    MouseMode = GetNextValue();
                    break;
                case "--mouse-bind":
                    MouseBind = GetNextValue();
                    break;
                case "--shortcut-mod":
                    ShortcutMod = GetNextValue();
                    break;
                case "--gamepad":
                    GamepadMode = GetNextValue();
                    break;
                case "--record":
                    RecordFile = GetNextValue();
                    break;
                case "--record-format":
                    RecordFormat = GetNextValue();
                    break;
                case "--record-orientation":
                    RecordOrientation = GetNextValue();
                    break;
                case "--new-display":
                    var ndVal = GetNextValue();
                    NewDisplay = string.IsNullOrWhiteSpace(ndVal) ? Features.VirtualDisplay.NewDisplay.AutoResolutionValue : ndVal;
                    break;
                case "--display-ime-policy":
                    DisplayImePolicy = GetNextValue();
                    break;
                case "--push-target":
                    PushTarget = GetNextValue();
                    break;
                case "-p":
                case "--port":
                    PortRange = GetNextValue();
                    break;
                case "--tunnel-host":
                    TunnelHost = GetNextValue();
                    break;
                case "--tunnel-port":
                    TunnelPort = GetNextValue();
                    break;
                case "--pause-on-exit":
                    PauseOnExit = val ?? "true";
                    break;
                case "-v":
                case "--verbosity":
                    Verbosity = GetNextValue();
                    break;

                // Booleans
                case "--turn-screen-off":
                    TurnScreenOff = true;
                    break;
                case "-w":
                case "--stay-awake":
                    StayAwake = true;
                    break;
                case "-t":
                case "--show-touches":
                    ShowTouches = true;
                    break;
                case "--power-off-on-close":
                    PowerOffOnClose = true;
                    break;
                case "--no-power-on":
                    NoPowerOn = true;
                    break;
                case "--keep-active":
                    KeepActive = true;
                    break;
                case "-f":
                case "--fullscreen":
                    Fullscreen = true;
                    break;
                case "--always-on-top":
                    AlwaysOnTop = true;
                    break;
                case "--window-borderless":
                    Borderless = true;
                    break;
                case "--disable-screensaver":
                    DisableScreensaver = true;
                    break;
                case "--no-window-aspect-ratio-lock":
                    NoWindowAspectRatioLock = true;
                    break;
                case "--no-terminal-title":
                    NoTerminalTitle = true;
                    break;
                case "--no-window":
                    NoWindow = true;
                    break;
                case "--no-video":
                    NoVideo = true;
                    break;
                case "--print-fps":
                    PrintFps = true;
                    break;
                case "--no-video-playback":
                    NoVideoPlayback = true;
                    break;
                case "--no-downsize-on-error":
                    NoDownsizeOnError = true;
                    break;
                case "--no-mipmaps":
                    NoMipmaps = true;
                    break;
                case "--ignore-video-encoder-constraints":
                    IgnoreVideoEncoderConstraints = true;
                    break;
                case "--no-audio":
                    NoAudio = true;
                    break;
                case "--no-audio-playback":
                    NoAudioPlayback = true;
                    break;
                case "--audio-dup":
                    AudioDup = true;
                    break;
                case "--require-audio":
                    RequireAudio = true;
                    break;
                case "--video-source":
                    if (val?.Equals("camera", StringComparison.OrdinalIgnoreCase) == true)
                        CameraSource = true;
                    break;
                case "--camera-torch":
                    CameraTorch = true;
                    break;
                case "--camera-high-speed":
                    CameraHighSpeed = true;
                    break;
                case "-n":
                case "--no-control":
                    NoControl = true;
                    break;
                case "--otg":
                    OtgMode = true;
                    break;
                case "--prefer-text":
                    PreferText = true;
                    break;
                case "--raw-key-events":
                    RawKeyEvents = true;
                    break;
                case "--no-key-repeat":
                    NoKeyRepeat = true;
                    break;
                case "--no-mouse-hover":
                    NoMouseHover = true;
                    break;
                case "--legacy-paste":
                    LegacyPaste = true;
                    break;
                case "--no-clipboard-autosync":
                    NoClipboardAutosync = true;
                    break;
                case "--no-playback":
                    NoPlayback = true;
                    break;
                case "--flex-display":
                    FlexDisplay = true;
                    break;
                case "--no-vd-system-decorations":
                    NoVdSystemDecorations = true;
                    break;
                case "--no-vd-destroy-content":
                    NoVdDestroyContent = true;
                    break;
                case "--tcpip":
                    var tcpVal = GetNextValue();
                    TcpIp = string.IsNullOrWhiteSpace(tcpVal) ? Features.DeviceAndConnection.TcpIp.AutoValue : tcpVal;
                    break;
                case "-d":
                case "--select-usb":
                    SelectUsb = true;
                    break;
                case "-e":
                case "--select-tcpip":
                    SelectTcpIp = true;
                    break;
                case "--force-adb-forward":
                    ForceAdbForward = true;
                    break;
                case "--no-cleanup":
                    NoCleanup = true;
                    break;
                case "--kill-adb-on-close":
                    KillAdbOnClose = true;
                    break;

                default:
                    unhandled.Add(token);
                    break;
            }
        }

        if (unhandled.Count > 0)
        {
            ExtraArgs = string.Join(" ", unhandled);
        }
    }

    private static List<string> ParseTokens(string command)
    {
        var tokens = new List<string>();
        if (string.IsNullOrWhiteSpace(command)) return tokens;

        bool inQuotes = false;
        char quoteChar = '\0';
        int start = -1;

        for (int i = 0; i < command.Length; i++)
        {
            char c = command[i];

            if (inQuotes)
            {
                if (c == quoteChar)
                {
                    inQuotes = false;
                }
            }
            else
            {
                if (c == '\'' || c == '"')
                {
                    inQuotes = true;
                    quoteChar = c;
                    if (start == -1) start = i;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (start != -1)
                    {
                        tokens.Add(command[start..i]);
                        start = -1;
                    }
                }
                else
                {
                    if (start == -1) start = i;
                }
            }
        }

        if (start != -1)
        {
            tokens.Add(command[start..]);
        }

        return tokens;
    }
}