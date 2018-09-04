using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public static class ServerHostBuilderExtensions
    {
        public static IServerHostBuilder ConfigureConfiguration(this IServerHostBuilder builder, Action<IConfigurationBuilder> configure)
        {
            var configBuilder = new ConfigurationBuilder();
            configure?.Invoke(configBuilder);
            builder.Services.AddSingleton<IConfiguration>(configBuilder.Build());
            return builder;
        }

        public static IServerHostBuilder ConfigureLog(this IServerHostBuilder builder, Action<ILoggingBuilder> configure)
        {
            return builder.ConfigureServices(i => i.AddLogging(configure));
        }
    }
}