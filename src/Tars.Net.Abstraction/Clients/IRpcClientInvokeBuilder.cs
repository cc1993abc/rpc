using AspectCore.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tars.Net.Clients.Proxy
{
    public interface IRpcClientInvokeBuilder
    {
        Func<AspectContext, AspectDelegate, Task> Build(MethodInfo item2);
    }
}