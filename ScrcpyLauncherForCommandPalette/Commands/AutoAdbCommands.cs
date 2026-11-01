// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette.Commands;

public sealed partial class AddToAutoAdbFromFavoriteCommand : InvokableCommand
{
    private readonly ScrcpyFavoriteCommand _favorite;

    public AddToAutoAdbFromFavoriteCommand(ScrcpyFavoriteCommand favorite)
    {
        _favorite = favorite;
        Name = "Use as AutoAdb";
        Icon = Classes.Icon.Flow;
    }

    public override CommandResult Invoke()
    {
        try
        {
            Logger.Log($"[AddToAutoAdbFromFavoriteCommand] Invoking for '{_favorite.Name}'");
            var sm = SettingsManager.Instance ?? ScrcpyRunner.SettingsManager;
            if (sm == null)
            {
                Logger.Log("[AddToAutoAdbFromFavoriteCommand] SettingsManager not available");
                new ToastStatusMessage("Settings not initialized.").Show();
                return CommandResult.KeepOpen();
            }

            var autoCmd = new AutoAdbCommand
            {
                Name = _favorite.Name,
                ScrcpyArguments = _favorite.Arguments,
                IsEnabled = true,
            };

            sm.AddAutoAdbCommand(autoCmd);
            new ToastStatusMessage($"Added '{_favorite.Name}' to AutoAdb (running in background)!").Show();
            Logger.Log($"[AddToAutoAdbFromFavoriteCommand] Successfully added '{_favorite.Name}'");
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[AddToAutoAdbFromFavoriteCommand] Exception: {ex.Message}");
            new ToastStatusMessage($"Error adding to AutoAdb: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }
}

public sealed partial class ToggleAutoAdbStatusCommand : InvokableCommand
{
    private readonly string _id;
    private readonly bool _targetState;
    private readonly string _name;

    public ToggleAutoAdbStatusCommand(string id, bool targetState, string name)
    {
        _id = id;
        _targetState = targetState;
        _name = name;
        Name = targetState ? "Start Monitoring" : "Stop Monitoring";
        Icon = targetState ? new IconInfo("\uE768") : new IconInfo("\uE71A");
    }

    public override CommandResult Invoke()
    {
        try
        {
            Logger.Log($"[ToggleAutoAdbStatusCommand] Invoking for '{_name}' (ID: {_id}) to targetState: {_targetState}");
            var sm = SettingsManager.Instance ?? ScrcpyRunner.SettingsManager;
            sm?.ToggleAutoAdbCommand(_id, _targetState);

            var status = _targetState ? "Started" : "Stopped";
            new ToastStatusMessage($"{status} AutoAdb monitoring for '{_name}'.").Show();
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[ToggleAutoAdbStatusCommand] Exception: {ex.Message}");
            new ToastStatusMessage($"Error toggling AutoAdb: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }
}

public sealed partial class RemoveAutoAdbItemCommand : InvokableCommand
{
    private readonly string _id;
    private readonly string _name;

    public RemoveAutoAdbItemCommand(string id, string name)
    {
        _id = id;
        _name = name;
        Name = "Remove from AutoAdb";
        Icon = Classes.Icon.EraseTool;
    }

    public override CommandResult Invoke()
    {
        try
        {
            Logger.Log($"[RemoveAutoAdbItemCommand] Invoking remove for '{_name}' (ID: {_id})");
            var sm = SettingsManager.Instance ?? ScrcpyRunner.SettingsManager;
            sm?.RemoveAutoAdbCommand(_id);

            new ToastStatusMessage($"Removed '{_name}' from AutoAdb and stopped process.").Show();
            Logger.Log($"[RemoveAutoAdbItemCommand] Completed remove for '{_name}' (ID: {_id})");
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            Logger.Log($"[RemoveAutoAdbItemCommand] Exception: {ex.Message}");
            new ToastStatusMessage($"Error removing from AutoAdb: {ex.Message}").Show();
            return CommandResult.KeepOpen();
        }
    }
}