using AspectCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tars.Net.Hosting
{
    public interface IServerHostBuilder
    {
        IServerHostBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configure);

        IServerHostBuilder ConfigureServices(Action<IServiceCollection> configure);

        IServerHostBuilder ConfigureAop(Action<IAspectConfiguration> configure);

        IServerHost Build();
    }
}