using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public class ServerHostBuilder : IServerHostBuilder
    {
        public IServiceCollection Services { get; }

        public IConfigurationBuilder ConfigurationBuilder { get; }

        public ServerHostBuilder()
        {
            Services = new ServiceCollection();
            ConfigurationBuilder = new ConfigurationBuilder();
        }

        public virtual IServerHost Build()
        {
            return Services
                .AddSingleton<IConfiguration>(ConfigurationBuilder.Build())
                .BuildServiceProvider()
                .GetRequiredService<IServerHost>();
        }
    }
}