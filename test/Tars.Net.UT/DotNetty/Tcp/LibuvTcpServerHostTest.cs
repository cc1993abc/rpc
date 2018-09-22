using DotNetty.Buffers;
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
using Tars.Net.Hosting.Tcp;
using Xunit;

namespace Tars.Net.UT.DotNetty.Tcp
{
    public class LibuvTcpServerHostTest
    {
        [Fact]
        public async Task RunHostShouldBeNoError()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));
            builder.Object.UseLibuvTcpHost();
            services.AddSingleton(new Mock<IDecoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IEncoder<IByteBuffer>>().Object);
            services.AddSingleton(new Mock<IClientCallBack>().Object);
            services.AddSingleton(new Mock<ILogger<LibuvTcpServerHost>>().Object);
            services.AddSingleton(new RpcConfiguration()
            {
                ClientConfig = new Dictionary<string, ClientConfiguration>(StringComparer.OrdinalIgnoreCase)
                 {
                     { "Tcp",  new ClientConfiguration()}
                 }
            });
            services.AddLibuvTcpClient();
            var host = services.BuildServiceProvider().GetRequiredService<IHostedService>();
            Assert.NotNull(((LibuvTcpServerHost)host).Provider);
            await host.StartAsync(CancellationToken.None);
            await host.StopAsync(CancellationToken.None);
        }
    }
}