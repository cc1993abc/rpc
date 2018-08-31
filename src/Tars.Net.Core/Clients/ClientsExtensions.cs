using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public static class ClientsExtensions
    {
        public static IServiceCollection ReigsterRpcClients(this IServiceCollection services, params Assembly[] assemblies)
        {
            var rpcMetadata = services.GetRpcMetadata();
            foreach (var client in rpcMetadata.Clients)
            {
                var type = client.GetReflector().GetMemberInfo().AsType();
                services.TryAddSingleton(type, j =>
                {
                    return j.GetRequiredService<IClientProxyCreator>().Create(type);
                });
            }
            services.TryAddSingleton<IRpcClientFactory, RpcClientFactory>();
            services.TryAddSingleton<IClientCallBack, ClientCallBack>();
            return services;
        }
    }
}