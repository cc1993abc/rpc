using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public static class ServerHostBuilderExtensions
    {
        public static IServiceCollection ReigsterRpcServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            var rpcMetadata = services.GetRpcMetadata();
            foreach (var (service, implementation) in rpcMetadata.RpcServices)
            {
                services.TryAddSingleton(service.GetReflector().GetMemberInfo().AsType(), implementation.GetReflector().GetMemberInfo().AsType());
            }

            services.TryAddSingleton<IServerInvoker, ServerInvoker>();
            services.TryAddSingleton<IServerHandler, ServerHandler>();
            return services;
        }
    }
}