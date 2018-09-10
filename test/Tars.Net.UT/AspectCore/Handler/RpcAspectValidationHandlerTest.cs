using AspectCore.DynamicProxy;
using Tars.Net.Clients;
using Tars.Net.Extensions.AspectCore;
using Tars.Net.UT.Core.Hosting;
using Xunit;

namespace Tars.Net.UT.AspectCore.Handler
{
    public class RpcAspectValidationHandlerTest
    {
        private readonly RpcAspectValidationHandler sut;

        public RpcAspectValidationHandlerTest()
        {
            sut = new RpcAspectValidationHandler();
        }

        [Fact]
        public void OrderShouldBe0()
        {
            Assert.Equal(-1, sut.Order);
        }

        [Fact]
        public void ValidationWhenRpcMethodShouldBeTrue()
        {
            var context = new AspectValidationContext
            {
                Method = typeof(ITestServer).GetMethods()[0]
            };
            Assert.True(sut.Invoke(context, i => false));
        }

        [Fact]
        public void ValidationWhenNotRpcMethodShouldBeFalse()
        {
            var context = new AspectValidationContext
            {
                Method = typeof(IClientCallBack).GetMethods()[0]
            };
            Assert.False(sut.Invoke(context, i => false));
        }
    }
}