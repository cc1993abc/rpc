using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Clients.Proxy;

namespace Tars.Net.Clients
{
    public static partial class ClientsExtensions
    {
        public static IServiceCollection ReigsterRpcClients(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.ReigsterRpcDep();
            var all = RpcExtensions.GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = RpcExtensions.GetAllRpcServicesAndClients(all);
            foreach (var client in rpcClients)
            {
                var type = client.GetReflector().GetMemberInfo().AsType();
                services.TryAddSingleton(type, j =>
                {
                    return j.GetRequiredService<IClientProxyCreater>().Create(type);
                });
            }
            services.TryAddSingleton<IRpcClientInvokerFactory>(j => new RpcClientInvokerFactory(rpcClients, j.GetRequiredService<IRpcClientFactory>()));
            return services;
        }
    }
}