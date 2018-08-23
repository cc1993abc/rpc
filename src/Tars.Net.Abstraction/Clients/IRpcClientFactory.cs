using System.Reflection;

namespace Tars.Net.Clients
{
    public interface IRpcClientFactory
    {
        IRpcClientInvoker GetClientInvoker(MethodInfo method);
    }
}