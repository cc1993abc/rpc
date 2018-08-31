using System.Reflection;
using Tars.Net.Internal;
using System;

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
            MethodInfo mi = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create), new Type[] { });
            mi = mi.MakeGenericMethod(type, typeof(ClientProxy));
            var clientProxy = mi.Invoke(null, null);

            ((ClientProxy)clientProxy).SetClientFactory(rpcClientFactory);
            return clientProxy;
        }
    }
}