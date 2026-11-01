// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Features.Diagnostic;
using ScrcpyLauncherForCommandPalette.Helpers;

namespace ScrcpyLauncherForCommandPalette.Services;

public sealed class ScrcpyCommandBuilderService
{
    private static readonly Lazy<ScrcpyCommandBuilderService> _lazy = new(() => new ScrcpyCommandBuilderService());
    public static ScrcpyCommandBuilderService Instance => _lazy.Value;

    public ScrcpyCommandBuilderState State { get; private set; } = new();

    public string Command
    {
        get => State.FinalCommand;
        set => State.FinalCommand = value;
    }

    public ScrcpyCommandBuilderService()
    {
        Reset();
    }

    public void Initialize(DeviceInfo? targetDevice = null, ScrcpyFavoriteCommand? existingFavorite = null)
    {
        State.Reset(targetDevice);

        if (existingFavorite != null)
        {
            State.FavoriteName = existingFavorite.Name;
            State.LoadFromArguments(existingFavorite.Arguments);

            if (!string.IsNullOrEmpty(existingFavorite.TargetDeviceSerial) && string.IsNullOrEmpty(State.DeviceSerial))
            {
                State.DeviceSerial = existingFavorite.TargetDeviceSerial;
            }
        }
        else if (targetDevice == null)
        {
            try
            {
                var devices = AdbHelper.GetConnectedDevices();
                if (devices.Length > 0 && string.IsNullOrEmpty(State.DeviceSerial))
                {
                    State.DeviceSerial = devices[0].Serial;
                }
            }
            catch
            {
            }
        }

        string serial = targetDevice?.Serial ?? State.DeviceSerial;
        if (ListApps.TryGetCachedApps(serial, out var cached) && cached.Count > 0)
        {
            State.InstalledApps = cached;
        }

        RebuildFinalCommand();
    }

    public void RebuildFinalCommand()
    {
        State.FinalCommand = ScrcpyCommandBuilderState.BuildFullCommand(State);
    }

    public void Reset(DeviceInfo? targetDevice = null)
    {
        State.Reset(targetDevice);
        RebuildFinalCommand();
    }
}