// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Avalonia.Extensions.Hosting
{
    /// <summary>
    /// A non-buildable <see cref="IHostBuilder"/> for <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.
    /// Use <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}.Build"/> to build the <see cref="AvaloniaApplicationBuilder{TApplication,TWindow}"/>.
    /// </summary>
    public sealed class ConfigureHostBuilder : IHostBuilder
    {
        private readonly ConfigurationManager _configuration;
        private readonly IServiceCollection _services;
        private readonly HostBuilderContext _context;
        private AppBuilder _appBuilder;

        private readonly List<Action<IHostBuilder>> _operations = new();

        internal ConfigureHostBuilder(HostBuilderContext context, ConfigurationManager configuration,
            IServiceCollection services, AppBuilder appBuilder)
        {
            _configuration = configuration;
            _services = services;
            _context = context;
            _appBuilder = appBuilder;
        }

        /// <inheritdoc />
        public IDictionary<object, object> Properties => _context.Properties;

        IHost IHostBuilder.Build()
        {
            throw new NotSupportedException();
        }

        public IHostBuilder ConfigureAvaloniaAppBuilder(
            Func<HostBuilderContext, AppBuilder, AppBuilder> configureAvaloniaAppBuilder)
        {
            // Run these immediately so that they are observable by the imperative code
            _appBuilder = configureAvaloniaAppBuilder(_context, _appBuilder);
            return this;
        }

        /// <inheritdoc />
        public IHostBuilder ConfigureAppConfiguration(
            Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            // Run these immediately so that they are observable by the imperative code
            configureDelegate(_context, _configuration);
            return this;
        }

        /// <inheritdoc />
        public IHostBuilder ConfigureContainer<TContainerBuilder>(
            Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            if (configureDelegate is null)
            {
                throw new ArgumentNullException(nameof(configureDelegate));
            }

            _operations.Add(b => b.ConfigureContainer(configureDelegate));
            return this;
        }

        /// <inheritdoc />
        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            var previousApplicationName = _configuration[HostDefaults.ApplicationKey];
            // Use the real content root so we can compare paths
            var previousEnvironment = _configuration[HostDefaults.EnvironmentKey];

            // Run these immediately so that they are observable by the imperative code
            configureDelegate(_configuration);

            // Disallow changing any host settings this late in the cycle, the reasoning is that we've already loaded the default configuration
            // and done other things based on environment name, application name or content root.
            if (!string.Equals(previousApplicationName, _configuration[HostDefaults.ApplicationKey],
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(
                    $"The application name changed from \"{previousApplicationName}\" to \"{_configuration[HostDefaults.ApplicationKey]}\". Changing the host configuration using AvaloniaApplicationBuilder.Host is not supported. Use AvaloniaApplication.CreateBuilder(AvaloniaApplicationOptions) instead.");
            }

            if (!string.Equals(previousEnvironment, _configuration[HostDefaults.EnvironmentKey],
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(
                    $"The environment changed from \"{previousEnvironment}\" to \"{_configuration[HostDefaults.EnvironmentKey]}\". Changing the host configuration using AvaloniaApplicationBuilder.Host is not supported. Use AvaloniaApplication.CreateBuilder(AvaloniaApplicationOptions) instead.");
            }

            return this;
        }

        /// <inheritdoc />
        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            // Run these immediately so that they are observable by the imperative code
            configureDelegate(_context, _services);
            return this;
        }

        /// <inheritdoc />
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
            IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _operations.Add(b => b.UseServiceProviderFactory(factory));
            return this;
        }

        /// <inheritdoc />
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
            Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
            where TContainerBuilder : notnull
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _operations.Add(b => b.UseServiceProviderFactory(factory));
            return this;
        }

        internal void RunDeferredCallbacks(IHostBuilder hostBuilder)
        {
            foreach (var operation in _operations)
            {
                operation(hostBuilder);
            }
        }
    }
}