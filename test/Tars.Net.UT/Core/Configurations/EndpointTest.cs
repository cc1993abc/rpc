using System;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Configurations;
using Xunit;

namespace Tars.Net.UT.Core.Configurations
{
    public class EndpointTest
    {
        [Fact]
        public void ValidationWhenTarsEndpointParseShouldBeTrue()
        {
            string tcpHost = "tcp -h 127.0.0.1 -p 19386 -t 60000";
            string udpHost = "udp -h 127.0.0.1 -p 19386 -t 60000";
            Assert.NotNull(Endpoint.Parse(tcpHost));
            Assert.NotNull(Endpoint.Parse(udpHost));
        }
    }
}
