using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerInvoker
    {
        Task<Response> InvokeAsync(Request req, Response resp);
    }
}