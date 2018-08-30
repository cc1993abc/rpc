using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public interface IServerHostBuilder
    {
        IServiceCollection Services { get; }

        IConfigurationBuilder ConfigurationBuilder { get; }

        IServerHost Build();
    }
}