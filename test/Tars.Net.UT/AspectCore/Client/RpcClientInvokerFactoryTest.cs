using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Metadata;
using Tars.Net.UT.Core.Hosting.RpcExtensionsUT;
using Xunit;

namespace Tars.Net.UT.AspectCore.Client
{
    public class RpcClientInvokerFactoryTest
    {
        private readonly ITestRpcInterface sut;

        public RpcClientInvokerFactoryTest()
        {
            var decoder = new Mock<IContentDecoder>();
            var clientFactory = new Mock<IRpcClientFactory>();
            clientFactory.Setup(i => i.SendAsync(It.IsAny<Request>()))
                .Returns<Request>(i =>
                {
                    var resp = i.CreateResponse();
                    if (i.Parameters[0].ToString() == "3")
                    {
                        resp.ReturnValue = 3;
                        return Task.FromResult(resp);
                    }
                    else if (i.Parameters[0].ToString() == "4")
                    {
                        resp.ReturnParameters = new object[0];
                        resp.ReturnValue = 4;
                        return Task.FromResult(resp);
                    }
                    else if (i.Parameters[0].ToString() == "5")
                    {
                        resp.ReturnParameters = new object[] { 6 };
                        resp.ReturnValue = 6;
                        return Task.FromResult(resp);
                    }
                    else if (i.Parameters[0].ToString() == "7")
                    {
                        return Task.FromResult(new Response());
                    }
                    else
                    {
                        return Task.FromResult(resp);
                    }
                });
            sut = new ServiceCollection()
                .AddSingleton(new RpcConfiguration())
                .AddSingleton(decoder.Object)
                .AddSingleton(clientFactory.Object)
                .ReigsterRpcClients()
                .AddAop()
                .BuildDynamicProxyServiceProvider()
                .GetRequiredService<ITestRpcInterface>();
        }

        [Fact]
        public void RpcClientInvokerWhenNoReturnParametersShouldBe3()
        {
            Assert.Equal(3, sut.Call(3));
        }

        [Fact]
        public void RpcClientInvokerWhenReturnParametersEmptyShouldBe4()
        {
            Assert.Equal(4, sut.Call(4));
        }

        [Fact]
        public void RpcClientInvokerWhenHasReturnParametersShouldBe6()
        {
            Assert.Equal(0, sut.CallOneway(out int a));
            Assert.Equal(0, a);
        }
    }
}