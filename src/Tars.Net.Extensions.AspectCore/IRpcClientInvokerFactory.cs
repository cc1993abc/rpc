using AspectCore.DynamicProxy;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Tars.Net.Extensions.AspectCore
{
    public interface IRpcClientInvokerFactory
    {
        Func<AspectContext, AspectDelegate, Task> GetClientInvoker(MethodInfo method);
    }
}