using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;

namespace Tars.Net.Extensions.AspectCore
{
    public class AspectCoreServerHostBuilder : IServerHostBuilder
    {
        public IServiceCollection Services { get; }

        public IConfigurationBuilder ConfigurationBuilder { get; }

        public AspectCoreServerHostBuilder()
        {
            Services = new ServiceCollection();
            ConfigurationBuilder = new ConfigurationBuilder();
        }

        public IServerHost Build()
        {
            return Services
                .ReigsterRpcDependency()
                .AddDynamicProxy(c =>
                {
                    c.ValidationHandlers.Add(new RpcAspectValidationHandler());
                })
                .AddSingleton<IConfiguration>(ConfigurationBuilder.Build())
                .BuildAspectCoreServiceProvider()
                .GetRequiredService<IServerHost>();
        }
    }
}