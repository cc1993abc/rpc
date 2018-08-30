using System;
using System.Collections.Generic;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Attributes;
using Tars.Net.Hosting;

namespace Tars.Net.Extensions.AspectCore
{
    public class AspectCoreServerHostBuilder : IServerHostBuilder
    {
        public IServiceCollection Services { get; }

        public IConfigurationBuilder ConfigurationBuilder { get; }

        public IEnumerable<Type> Clients { get; }

        public IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }

        public AspectCoreServerHostBuilder()
        {
            Services = new ServiceCollection();
            ConfigurationBuilder = new ConfigurationBuilder();
            var all = RpcHelper.GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = RpcHelper.GetAllRpcServicesAndClients(all);
            Clients = rpcClients;
            RpcServices = rpcServices;
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