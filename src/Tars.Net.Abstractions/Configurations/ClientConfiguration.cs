using System;
using System.Collections.Generic;
using System.Net;
using Tars.Net.Metadata;

namespace Tars.Net.Configurations
{
    public class ClientConfiguration
    {
        public RpcProtocol Protocol { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public int Timeout { get; set; } = 60;

        private EndPoint endPoint;

        public EndPoint EndPoint
        {
            get
            {
                if (endPoint == null)
                {
                    if (IPAddress.TryParse(Host, out IPAddress ip))
                    {
                        endPoint = new IPEndPoint(ip, Port);
                    }
                    else
                    {
                        endPoint = new DnsEndPoint(Host, Port);
                    }
                }
                return endPoint;
            }
        }
    }
}