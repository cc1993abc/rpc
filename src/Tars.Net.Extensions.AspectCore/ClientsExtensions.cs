using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Tars.Net.Extensions.AspectCore;

namespace Tars.Net.Clients
{
    public static class ClientsExtensions
    {
        public static IServiceCollection AddAop(this IServiceCollection services, Action<IAspectConfiguration> configure = null)
        {
            //replace the inner implement
            services.AddSingleton<IClientProxyCreator, AspectCoreClientProxyCreator>();
            services.AddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            services.AddSingleton<RpcClientInvokerFactory>();
            services.AddDynamicProxy(c =>
            {
                c.ValidationHandlers.Add(new RpcAspectValidationHandler());
                configure?.Invoke(c);
            });
            return services;
        }
    }
}