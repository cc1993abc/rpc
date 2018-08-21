using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using Tars.Csharp.Hosting.Configurations;
using Tars.Csharp.Hosting.Tcp;

namespace Tars.Csharp.Hosting
{
    public static class ServerHostExtensions
    {
        public static IServerHostBuilder UseLibuvTcp(this IServerHostBuilder builder)
        {
            return builder.ConfigureServices(i =>
            {
                i.TryAddSingleton<IServerHost, LibuvTcpServerHost>();
            });
        }

        public static IServerHostBuilder AddHostConfiguration(this IServerHostBuilder builder, string key = "Host")
        {
            return builder.ConfigureServices(i =>
            {
                i.TryAddSingleton(j =>
                {
                    HostConfiguration config = new HostConfiguration();
                    j.GetRequiredService<IConfiguration>().Bind(key, config);
                    return config;
                });
            });
        }

        public static IServerHostBuilder ConfigureLog(this IServerHostBuilder builder, Action<ILoggingBuilder> configure)
        {
            return builder.ConfigureServices(i => i.AddLogging(configure));
        }
    }
}