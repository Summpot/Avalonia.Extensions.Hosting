using Avalonia;
using Microsoft.Extensions.Hosting;
using SampleAppWithStartupUri;
using Avalonia.Extensions.Hosting;

var builder = AvaloniaApplication<App, MainWindow>.CreateBuilder(args);

builder.Host.ConfigureAvaloniaAppBuilder((context, appBuilder) =>
    appBuilder.UsePlatformDetect().WithInterFont().LogToTrace().SetupWithClassicDesktopLifetime(args));

var app = builder.Build();

await app.RunAsync();
