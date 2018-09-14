using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;
using Tars.Net.Hosting.Udp;
using Xunit;

namespace Tars.Net.UT.DotNetty.Udp
{
    public class UdpServerHostTest
    {
        [Fact]
        public async Task RunHostShouldBeNoError()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));
            builder.Object.UseUdpHost();
            services.AddSingleton(new Mock<IDecoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IEncoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IContentDecoder>().Object);
            services.AddSingleton(new Mock<IClientCallBack>().Object);
            services.AddSingleton(new Mock<ILogger<UdpServerHost>>().Object);
            services.AddSingleton(new RpcConfiguration()
            {
                 ClientConfig = new Dictionary<string, ClientConfiguration>(StringComparer.OrdinalIgnoreCase)
                 {
                     { "Tcp",  new ClientConfiguration()}
                 },
                 Port = 9999
            });
            services.AddUdpClient();
            var host = services.BuildServiceProvider().GetRequiredService<IHostedService>();
            Assert.NotNull(((UdpServerHost)host).Provider);
            await Assert.ThrowsAsync<ClosedChannelException>(() => host.StartAsync(CancellationToken.None));
            await host.StopAsync(CancellationToken.None);
        }
    }
}