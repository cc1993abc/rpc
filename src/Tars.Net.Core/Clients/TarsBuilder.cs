using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Attributes;

namespace Tars.Net.Clients
{
    public class TarsClientBuilder : ITarsClientBuilder
    {
        public IServiceCollection Services { get; }

        public IEnumerable<Type> Clients { get; }

        public TarsClientBuilder(IServiceCollection service)
        {
            this.Services = service;
            var all = RpcHelper.GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = RpcHelper.GetAllRpcServicesAndClients(all);
            Clients = rpcClients;
        }
    }
}