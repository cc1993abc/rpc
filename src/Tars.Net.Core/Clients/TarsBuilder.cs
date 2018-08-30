using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Attributes;

namespace Tars.Net
{
    public class TarsBuilder : ITarsBuilder
    {
        public IServiceCollection Services { get; }

        public IEnumerable<Type> Clients { get; }

        public IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }

        public TarsBuilder(IServiceCollection service)
        {
            this.Services = service;
            var all = RpcHelper.GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = RpcHelper.GetAllRpcServicesAndClients(all);
            Clients = rpcClients;
            RpcServices = rpcServices;
        }
    }
}