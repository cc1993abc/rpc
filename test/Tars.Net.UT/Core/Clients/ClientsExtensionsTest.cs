using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Clients;
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
                .ReigsterRpcClients()
                .AddAop()
                .BuildDynamicProxyServiceProvider()
                .GetRequiredService<ITestRpcInterface>();
            Assert.NotNull(client);
        }
    }
}