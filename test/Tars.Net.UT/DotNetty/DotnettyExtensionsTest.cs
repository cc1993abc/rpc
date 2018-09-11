using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using Tars.Net.Clients;
using Tars.Net.Clients.Tcp;
using Tars.Net.DotNetty;
using Tars.Net.DotNetty.Hosting;
using Tars.Net.Hosting.Tcp;
using Xunit;

namespace Tars.Net.UT.DotNetty
{
    public class DotnettyExtensionsTest
    {
        [Fact]
        public void TestAddLibuvTcpClient()
        {
            var services = new ServiceCollection()
                .AddLibuvTcpClient();
            Assert.Contains(services, i => i.ImplementationType == typeof(LibuvTcpClient)
                && i.ServiceType == typeof(IRpcClient));
        }

        [Fact]
        public void TestUseLibuvTcpHost()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));
            builder.Object.UseLibuvTcpHost();
            Assert.Contains(services, i => i.ImplementationType == typeof(DotNettyServerHandler)
                && i.ServiceType == typeof(DotNettyServerHandler));
            Assert.Contains(services, i => i.ImplementationType == typeof(LibuvTcpServerHost)
                && i.ServiceType == typeof(IHostedService));
        }
    }
}