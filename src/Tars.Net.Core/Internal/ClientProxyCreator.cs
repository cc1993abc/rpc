using System.Reflection;
using Tars.Net.Internal;
using System;
using ClientProxy = Tars.Net.Internal.ClientProxy;

namespace Tars.Net.Clients
{
    internal class ClientProxyCreator : IClientProxyCreator
    {
        private readonly IRpcClientFactory rpcClientFactory;

        public ClientProxyCreator(IRpcClientFactory rpcClientFactory)
        {
            this.rpcClientFactory = rpcClientFactory;
        }

        public object Create(Type type)
        {
            return ClientProxy.Create(type, rpcClientFactory);
        }
    }
}