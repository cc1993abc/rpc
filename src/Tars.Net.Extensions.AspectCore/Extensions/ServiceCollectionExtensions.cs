using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tars.Net.Clients;

namespace Tars.Net.Extensions.AspectCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ReigsterRpcDependency(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientProxyCreater, AspectCoreClientProxyCreater>();
            services.TryAddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            return services;
        }
    }
}