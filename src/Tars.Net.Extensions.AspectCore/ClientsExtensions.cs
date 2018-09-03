using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Tars.Net.Extensions.AspectCore;

namespace Tars.Net.Clients
{
    public static partial class ClientsExtensions
    {
        public const string Context_IsRpcClient = "#IsRpcClient";

        public static IServiceCollection AddAop(this IServiceCollection services, Action<IAspectConfiguration> configure = null)
        {
            services.TryAddSingleton<IClientProxyCreator, AspectCoreClientProxyCreator>();
            services.TryAddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            services.TryAddSingleton<RpcClientInvokerFactory>();
            services.AddDynamicProxy(c =>
            {
                c.ValidationHandlers.Add(new RpcAspectValidationHandler());
                configure?.Invoke(c);
            });
            return services;
        }

        public static bool IsRpcClient(this AspectContext context)
        {
            return context.AdditionalData.ContainsKey(Context_IsRpcClient);
        }
    }
}