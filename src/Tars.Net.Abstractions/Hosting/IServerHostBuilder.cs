using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public interface IServerHostBuilder
    {
        IServiceCollection Services { get; }

        IServerHost Build();
    }
}