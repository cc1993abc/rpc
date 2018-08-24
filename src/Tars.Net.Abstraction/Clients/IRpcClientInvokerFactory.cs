using AspectCore.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tars.Net.Clients
{
    public interface IRpcClientInvokerFactory
    {
        Func<AspectContext, AspectDelegate, Task> GetClientInvoker(MethodInfo method);
    }
}