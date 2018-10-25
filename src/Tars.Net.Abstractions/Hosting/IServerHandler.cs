using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerHandler
    {
        Task<Response> ProcessAsync(Request req);
    }
}