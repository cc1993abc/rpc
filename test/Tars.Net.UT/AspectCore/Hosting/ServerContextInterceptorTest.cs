using AspectCore.DynamicProxy;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Hosting;
using Xunit;

namespace Tars.Net.UT.AspectCore.Hosting
{
    public class ServerContextInterceptorTest
    {
        private readonly ServerContextInterceptor sut;

        public ServerContextInterceptorTest()
        {
            sut = new ServerContextInterceptor();
        }

        [Fact]
        public void OrderShouldBe0()
        {
            Assert.Equal(0, sut.Order);
        }

        [Fact]
        public async void SetContextShouldCopyRight()
        {
            var additionalData = new Dictionary<string, object>();
            var serverContext = new ServerContext();
            serverContext.Context.Add("old", "old");
            var aspectContext = new Mock<AspectContext>();
            aspectContext.Setup(i => i.AdditionalData).Returns(additionalData);
            additionalData.Add(AspectClientsExtensions.Context_IsRpcClient, "true");
            additionalData.Add("old", "new");
            additionalData.Add("id", "newid");
            ServerContext.Current = serverContext;
            await sut.Invoke(aspectContext.Object, (i) => Task.CompletedTask);
            Assert.False(serverContext.Context.ContainsKey(AspectClientsExtensions.Context_IsRpcClient));
            Assert.Equal("new", serverContext.Context["old"]);
            Assert.Equal("newid", serverContext.Context["id"]);
        }
    }
}
