// Copyright (c) 2026 tanchekwei
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using ScrcpyLauncherForCommandPalette.Classes;
using ScrcpyLauncherForCommandPalette.Pages;
using ScrcpyLauncherForCommandPalette.Services;

namespace ScrcpyLauncherForCommandPalette;

#if DEBUG
[Guid("b2d29c92-d9d0-420f-b3f9-476df2007a1c")]
#else
[Guid("788ca191-f727-42bc-b3e8-25d56b26082e")]
#endif
public sealed partial class ScrcpyLauncherForCommandPalette : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;
    private readonly ScrcpyLauncherForCommandPaletteCommandsProvider _provider;

    public ScrcpyLauncherForCommandPalette(ManualResetEvent extensionDisposedEvent)
    {
        try
        {
            _extensionDisposedEvent = extensionDisposedEvent;
            AppDomain.CurrentDomain.ProcessExit += (s, e) => AutoAdbService.Instance.StopAll();

            var settingsManager = new SettingsManager();
            ScrcpyRunner.Initialize(settingsManager);
            StaticHelpItems.Initialize(settingsManager);
            FavoriteCommandsService.Instance.RebuildCache(settingsManager.Favorites, settingsManager);

            var page = new ScrcpyLauncherPage(settingsManager);
            _provider = new ScrcpyLauncherForCommandPaletteCommandsProvider(settingsManager, page);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            throw;
        }
    }

    public object? GetProvider(ProviderType providerType)
    {
        try
        {
#if DEBUG
            using var logger = new TimeLogger();
#endif
            return providerType switch
            {
                ProviderType.Commands => _provider,
                _ => null,
            };
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
            return null;
        }
    }

    public void Dispose()
    {
        try
        {
            AutoAdbService.Instance.Dispose();
            _extensionDisposedEvent.Set();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex);
        }
    }
}