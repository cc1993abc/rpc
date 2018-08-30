using Microsoft.Extensions.Configuration;

namespace Tars.Net.Hosting
{
    public interface IServerHostBuilder : ITarsBuilder
    {
        IConfigurationBuilder ConfigurationBuilder { get; }

        IServerHost Build();
    }
}