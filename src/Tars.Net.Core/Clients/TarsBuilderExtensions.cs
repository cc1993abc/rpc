using System.Reflection;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tars.Net.Clients
{
    public static class TarsBuilderExtensions
    {
        public static ITarsClientBuilder ReigsterRpcClients(this ITarsClientBuilder builder, params Assembly[] assemblies)
        {
            var services = builder.Services;
            foreach (var client in builder.Clients)
            {
                var type = client.GetReflector().GetMemberInfo().AsType();
                services.TryAddSingleton(type, j =>
                {
                    return j.GetRequiredService<IClientProxyCreater>().Create(type);
                });
            }
            services.TryAddSingleton<IRpcClientFactory, RpcClientFactory>();
            services.TryAddSingleton<IClientCallBack, ClientCallBack>();

            return builder;
        }
    }
}