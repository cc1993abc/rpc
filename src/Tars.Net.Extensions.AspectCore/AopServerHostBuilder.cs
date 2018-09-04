using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;
using Tars.Net.Metadata;

namespace Tars.Net.Extensions.AspectCore
{
    public class AopServerHostBuilder : IServerHostBuilder
    {
        private readonly IServerHostBuilder builder;

        public AopServerHostBuilder(IServerHostBuilder builder)
        {
            this.builder = builder;
        }

        public IServiceCollection Services => builder.Services;

        public IServerHost Build()
        {
            var rpcMetadata = Services.GetRpcMetadata();
            return Services
                .AddDynamicProxy(config =>
                {
                    config.Interceptors.AddTyped<ServerContextInterceptor>(method =>
                    {
                        return rpcMetadata.IsRpcServiceType(method.DeclaringType);
                    });
                })
                .BuildAspectCoreServiceProvider()
                .GetRequiredService<IServerHost>();
        }
    }
}