using System.Net;
using Tars.Net.Metadata;

namespace Tars.Net.Configurations
{
    public class ClientConfiguration
    {
        public RpcProtocol Protocol { get; set; }

        public EndPoint EndPoint { get; set; }
    }
}