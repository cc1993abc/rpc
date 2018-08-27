using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Codecs;

namespace Tars.Net.Hosting
{
    public static class ServicesExtensions
    {
        public static IServiceCollection ReigsterRpcServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.ReigsterRpcDependency();
            var all = RpcExtensions.GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = RpcExtensions.GetAllRpcServicesAndClients(all);
            foreach (var (service, implementation) in rpcServices)
            {
                services.TryAddSingleton(service.GetReflector().GetMemberInfo().AsType(), implementation.GetReflector().GetMemberInfo().AsType());
            }
            services.TryAddSingleton<IServerInvoker>(j => new ServerInvoker(rpcServices, j, j.GetRequiredService<IDecoder>()));
            return services;
        }

        public static IServerHostBuilder ConfigureLog(this IServerHostBuilder builder, Action<ILoggingBuilder> configure)
        {
            return builder.ConfigureServices(i => i.AddLogging(configure));
        }
    }
}