using AspectCore.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tars.Net.Clients.Proxy
{
    public class RpcClientInvokeBuilder : IRpcClientInvokeBuilder
    {
        public Func<AspectContext, AspectDelegate, Task> Build(MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}