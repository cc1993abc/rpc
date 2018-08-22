using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tars.Net.Hosting
{
    public class ServerHostBuilder : IServerHostBuilder
    {
        private IServiceCollection services = new ServiceCollection();
        private readonly IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

        public IServerHost Build()
        {
            return services
                .AddSingleton<IConfiguration>(configurationBuilder.Build())
                .BuildAspectCoreServiceProvider()
                .GetRequiredService<IServerHost>();
        }

        public IServerHostBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configure)
        {
            configure?.Invoke(configurationBuilder);
            return this;
        }

        public IServerHostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            configure?.Invoke(services);
            return this;
        }

        public IServerHostBuilder ConfigureAop(Action<IAspectConfiguration> configure)
        {
            services.AddDynamicProxy(configure);
            return this;
        }
    }
}