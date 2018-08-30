using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public interface IServerHostBuilder
    {
        IServiceCollection Services { get; }

        IConfigurationBuilder ConfigurationBuilder { get; }

        IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }

        IServerHost Build();
    }
}