using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public class ServerHostBuilder : IServerHostBuilder
    {
        public IServiceCollection Services { get; }

        public ServerHostBuilder()
        {
            Services = new ServiceCollection();
        }

        public virtual IServerHost Build()
        {
            return Services
                .BuildServiceProvider()
                .GetRequiredService<IServerHost>();
        }
    }
}