using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;
using Xunit;

namespace Tars.Net.UT.DotNetty.Tcp
{
    public class LibuvTcpClientTest
    {
        [Fact]
        public async void LibuvTcpClientRunShouldBeNoError()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new Mock<IDecoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IEncoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IClientCallBack>().Object);
            services.AddSingleton(new RpcConfiguration()
            {
                ClientConfig = new Dictionary<string, ClientConfiguration>(StringComparer.OrdinalIgnoreCase)
                 {
                     { "Tcp",  new ClientConfiguration()}
                 }
            });
            services.AddLibuvTcpClient();
            var client = services.BuildServiceProvider().GetRequiredService<IRpcClient>();
            await Assert.ThrowsAsync<ConnectException>(() => client.SendAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 333), new Metadata.Request()));
            await Assert.ThrowsAsync<ConnectException>(() => client.SendAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 333), new Metadata.Request()));
            await client.ShutdownGracefullyAsync(TimeSpan.Zero, TimeSpan.Zero);
        }
    }
}