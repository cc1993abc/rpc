using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;

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

        public IConfigurationBuilder ConfigurationBuilder => builder.ConfigurationBuilder;

        public IServerHost Build()
        {
            return Services
                .AddSingleton<IConfiguration>(ConfigurationBuilder.Build())
                .BuildAspectCoreServiceProvider()
                .GetRequiredService<IServerHost>();
        }
    }
}