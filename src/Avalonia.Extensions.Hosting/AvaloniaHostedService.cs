// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Hosting;

namespace Avalonia.Extensions.Hosting;

internal class AvaloniaHostedService<TApplication, TWindow> : IHostedService
    where TApplication : Application
    where TWindow : Window
{
    private readonly TApplication _application;
    private readonly TWindow? _window;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly AvaloniaApplicationOptions _options;

    public AvaloniaHostedService(TApplication application, TWindow? window,
        IHostApplicationLifetime hostApplicationLifetime, AvaloniaApplicationOptions options)
    {
        _application = application;
        _window = window;
        _hostApplicationLifetime = hostApplicationLifetime;
        _options = options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_application.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _window;
            desktop.Start(_options.Args ?? []);
        }

        _hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}