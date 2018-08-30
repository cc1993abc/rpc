using System;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tars.Net.Clients;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;

namespace Tars.Net.Extensions.AspectCore
{
    public static class ServerHostExtensions
    {
        public static IServerHostBuilder ConfigureAop(this IServerHostBuilder builder, Action<IAspectConfiguration> configure)
        {
            builder.Services.TryAddSingleton<IRpcClientInvokerFactory>(j => new RpcClientInvokerFactory(builder.Clients, j.GetRequiredService<IRpcClientFactory>()));
            builder.Services.AddDynamicProxy(configure);
            return builder;
        }
    }
}