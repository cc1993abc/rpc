using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class AopServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly Action<IAspectConfiguration> configure;

        public AopServiceProviderFactory(Action<IAspectConfiguration> configure)
        {
            this.configure = configure;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var rpcMetadata = containerBuilder.GetRpcMetadata();
            return containerBuilder
                .ConfigureDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<ServerContextInterceptor>(method =>
                    {
                        return rpcMetadata.IsRpcServiceType(method.DeclaringType);
                    });
                    configure?.Invoke(config);
                })
                .BuildDynamicProxyServiceProvider();
        }
    }
}