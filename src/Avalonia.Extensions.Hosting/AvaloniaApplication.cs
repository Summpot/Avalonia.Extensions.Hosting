// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Avalonia.Extensions.Hosting
{
    /// <summary>
    /// The web application used to configure the HTTP pipeline, and routes.
    /// </summary>
    public sealed class AvaloniaApplication<TApplication, TWindow> : IHost, IAsyncDisposable
        where TApplication : Application, new()
        where TWindow : Window
    {
        private bool _isLoaded;
        private readonly IHost _host;

        internal AvaloniaApplication(IHost host)
        {
            _host = host;
            Logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(Environment.ApplicationName);
        }

        public event EventHandler<ApplicationStartupEventArgs<TApplication, TWindow>>? Startup;
        public event EventHandler<ApplicationLoadedEventArgs<TApplication, TWindow>>? Loaded;

        /// <summary>
        /// The application's configured services.
        /// </summary>
        public IServiceProvider Services => _host.Services;

        /// <summary>
        /// The application's configured <see cref="IConfiguration"/>.
        /// </summary>
        public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();

        /// <summary>
        /// The application's configured <see cref="IHostEnvironment"/>.
        /// </summary>
        public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();

        /// <summary>
        /// Allows consumers to be notified of application lifetime events.
        /// </summary>
        public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();

        /// <summary>
        /// The default logger for the application.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaApplication{TApplication,TWindow}"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>The <see cref="AvaloniaApplication{TApplication,TWindow}"/>.</returns>
        public static AvaloniaApplication<TApplication, TWindow> Create(string[]? args = null) =>
            new AvaloniaApplicationBuilder<TApplication, TWindow>(new() { Args = args }).Build();

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/> class with preconfigured defaults.
        /// </summary>
        /// <returns>The <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.</returns>
        public static AvaloniaApplicationBuilder<TApplication, TWindow> CreateBuilder() =>
            new(new());

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>The <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.</returns>
        public static AvaloniaApplicationBuilder<TApplication, TWindow> CreateBuilder(string[] args) =>
            new(new() { Args = args });

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/> class with preconfigured defaults.
        /// </summary>
        /// <param name="options">The <see cref="AvaloniaApplicationOptions"/> to configure the <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.</param>
        /// <returns>The <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.</returns>
        public static AvaloniaApplicationBuilder<TApplication, TWindow> CreateBuilder(
            AvaloniaApplicationOptions options) =>
            new(options);

        /// <summary>
        /// Start the application.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A <see cref="Task"/> that represents the startup of the <see cref="AvaloniaApplication{TApplication,TWindow}"/>.
        /// Successful completion indicates the HTTP server is ready to accept new requests.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var application = Services.GetRequiredService<TApplication>();
            return _host.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A <see cref="Task"/> that represents the shutdown of the <see cref="AvaloniaApplication{TApplication,TWindow}"/>.
        /// Successful completion indicates that all the HTTP server has stopped.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken = default) =>
            _host.StopAsync(cancellationToken);

        /// <summary>
        /// Disposes the application.
        /// </summary>
        void IDisposable.Dispose() => _host.Dispose();

        /// <summary>
        /// Disposes the application.
        /// </summary>
        public ValueTask DisposeAsync() => ((IAsyncDisposable)_host).DisposeAsync();
    }
}