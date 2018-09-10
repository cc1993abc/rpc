using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Hosting;
using Tars.Net.UT.Core.Hosting.RpcExtensionsUT;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    public class ServerHostBuilderExtensionsTest
    {
        [Fact]
        public void ReigsterRpcServicesShouldBeRight()
        {
            var service = new ServiceCollection()
                .ReigsterRpcServices()
                .ConfigureDynamicProxy()
                .BuildDynamicProxyServiceProvider()
                .GetRequiredService<ITestAttributeTypeScan>();
            Assert.NotNull(service);
        }
    }
}