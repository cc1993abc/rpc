using DotNetty.Buffers;
using System;
using System.Net;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IRpcClient
    {
        RpcProtocol Protocol { get; }

        void SetClientCallBack(IClientCallBack clientCallBack);

        Task SendAsync(EndPoint endPoint, IByteBuffer request);

        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout);
    }
}