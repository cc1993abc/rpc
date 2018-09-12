using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.UT.Core.Hosting.RpcExtensionsUT;
using Xunit;

namespace Tars.Net.UT.Core.Clients
{
    public class ClientsExtensionsTest
    {
        [Fact]
        public void ReigsterRpcClientsShouldBeRight()
        {
            var client = new ServiceCollection()
                .AddSingleton(new RpcConfiguration())
                .AddSingleton(new Mock<IContentDecoder>().Object)
                .ReigsterRpcClients()
                .AddAop()
                .BuildDynamicProxyServiceProvider()
                .GetRequiredService<ITestRpcInterface>();
            Assert.NotNull(client);
        }
    }
}