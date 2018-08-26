using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using Tars.Net.Configurations;
using Tars.Net.Hosting.Tcp;

namespace Tars.Net.Hosting
{
    public static class ServerHostExtensions
    {
        public static IServerHostBuilder UseLibuvTcpHost(this IServerHostBuilder builder)
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
                    var config = new RpcConfiguration();
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