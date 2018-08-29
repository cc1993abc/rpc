using System;
using System.Net;
using System.Threading.Tasks;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public interface IRpcClient
    {
        RpcProtocol Protocol { get; }

        Task SendAsync(EndPoint endPoint, Request request);

        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout);
    }
}