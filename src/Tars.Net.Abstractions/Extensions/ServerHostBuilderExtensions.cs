using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;

namespace Tars.Net
{
    public static class ServerHostBuilderExtensions
    {
        public static IServerHostBuilder ConfigureConfiguration(this IServerHostBuilder builder, Action<IConfigurationBuilder> configure)
        {
            configure?.Invoke(builder.ConfigurationBuilder);
            return builder;
        }

        public static IServerHostBuilder ConfigureServices(this IServerHostBuilder builder, Action<IServiceCollection> configure)
        {
            configure?.Invoke(builder.Services);
            return builder;
        }
    }
}