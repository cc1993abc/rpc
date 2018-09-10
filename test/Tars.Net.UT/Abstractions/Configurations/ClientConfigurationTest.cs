using Tars.Net.Configurations;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Abstractions.Configurations
{
    public class ClientConfigurationTest
    {
        [Fact]
        public void WhenIpHostShouldBeIPEndPoint()
        {
            var result = new ClientConfiguration()
            {
                Host = "127.0.0.1",
                Port = 999
            }.EndPoint;
            Assert.Equal("127.0.0.1:999", result.ToString());
        }

        [Fact]
        public void WhenDnsHostShouldBeDnsEndPoint()
        {
            var result = new ClientConfiguration()
            {
                Host = "localhost",
                Port = 777
            }.EndPoint;
            Assert.Equal("Unspecified/localhost:777", result.ToString());
        }

        [Fact]
        public void DefaultProtocolShouldBeTcp()
        {
            var result = new ClientConfiguration().Protocol;
            Assert.Equal(RpcProtocol.Tcp, result);
        }

        [Fact]
        public void DefaultTimeoutShouldBe60()
        {
            var result = new ClientConfiguration().Timeout;
            Assert.Equal(60, result);
        }
    }
}