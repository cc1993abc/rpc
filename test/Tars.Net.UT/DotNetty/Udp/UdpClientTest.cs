using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;
using Xunit;

namespace Tars.Net.UT.DotNetty.Udp
{
    public class UdpClientTest
    {
        [Fact]
        public async Task UdpClientRunShouldBeNoError()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new Mock<IDecoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IEncoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IClientCallBack>().Object);
            services.AddSingleton(new RpcConfiguration()
            {
                ClientConfig = new Dictionary<string, ClientConfiguration>(StringComparer.OrdinalIgnoreCase)
                 {
                     { "Udp",  new ClientConfiguration()}
                 }
            });
            services.AddUdpClient();
            var client = services.BuildServiceProvider().GetRequiredService<IRpcClient>();
            await Assert.ThrowsAsync<ClosedChannelException>(() => client.SendAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 333), new Metadata.Request()));
            await Assert.ThrowsAsync<ClosedChannelException>(() => client.SendAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 333), new Metadata.Request()));
            await client.ShutdownGracefullyAsync(TimeSpan.Zero, TimeSpan.Zero);
        }
    }
}