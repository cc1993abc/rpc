using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IRpcClientFactory
    {
        Task<Response> SendAsync(Request req);
    }
}