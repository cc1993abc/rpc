using System;
using System.Reflection;

namespace Tars.Net.Clients
{
    public class RpcClientFactory : IRpcClientFactory
    {
        public IRpcClientInvoker GetClientInvoker(MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}