using DotNetty.Buffers;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IRpcClient
    {
        RpcProtocol Protocol { get; }

        Task SendAsync(IByteBuffer request);
    }
}