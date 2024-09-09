using System;
using Avalonia;
using Avalonia.Extensions.Hosting;
using GettingStarted;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// Create a builder by specifying the application and main window.
var builder =
    AvaloniaApplication<App, MainWindow>.CreateBuilder(args);

builder.Host.ConfigureAvaloniaAppBuilder((context, appBuilder) =>
    appBuilder.UsePlatformDetect().WithInterFont().LogToTrace().SetupWithClassicDesktopLifetime(args));
// Register the ViewModel to be injected into MainWindow to the DI container.
builder.Services.AddTransient<MainWindowViewModel>();

// Build and run the application.
var app = builder.Build();
await app.RunAsync();