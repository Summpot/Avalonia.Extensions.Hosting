﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Avalonia.Controls;

namespace Avalonia.Extensions.Hosting;

/// <summary>
/// Argument for the Avalonia application loaded event.
/// </summary>
/// <typeparam name="TApplication"></typeparam>
/// <typeparam name="TWindow"></typeparam>
public class ApplicationLoadedEventArgs<TApplication, TWindow> : EventArgs
    where TApplication : Application
    where TWindow : Window
{
    public ApplicationLoadedEventArgs(TApplication application, TWindow window)
    {
        Application = application;
        Window = window;
    }

    public TApplication Application { get; }
    public TWindow Window { get; }
}