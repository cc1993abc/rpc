using DotNetty.Transport.Channels;
using Moq;
using Tars.Net.Clients;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.DotNetty.Clients
{
    public class DotNettyClientHandlerTest
    {
        [Fact]
        public void TestDecoDotNettyClientHandler()
        {
            object result = null;
            var mockCallBack = new Mock<IClientCallBack>();
            mockCallBack.Setup(i => i.CallBack(It.IsAny<Response>()))
                .Callback<Response>(i =>
                {
                    result = i;
                });
            var context = new Mock<IChannelHandlerContext>();
            var handler = new DotNettyClientHandler(mockCallBack.Object);
            Assert.True(handler.IsSharable);
            handler.ChannelRead(context.Object, new Response());
            Assert.NotNull(result);
            Assert.IsType<Response>(result);
        }
    }
}