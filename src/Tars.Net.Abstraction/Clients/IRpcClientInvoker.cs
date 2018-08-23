using AspectCore.DynamicProxy;
using System.Threading.Tasks;

namespace Tars.Net.Clients
{
    public interface IRpcClientInvoker
    {
        Task InvokeAsync(AspectContext context);
    }
}