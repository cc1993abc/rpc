using DotNetty.Transport.Channels;
using Moq;
using System.Threading.Tasks;
using Tars.Net.DotNetty.Hosting;
using Tars.Net.Hosting;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.DotNetty.Hosting
{
    public class DotNettyServerHandlerTest
    {
        [Fact]
        public void TestDecoDotNettyClientHandler()
        {
            object result = null;
            var mockServerHandler = new Mock<IServerHandler>();
            mockServerHandler.Setup(i => i.Process(It.IsAny<Request>()))
                .Returns<Request>(i =>
                {
                    return i.CreateResponse();
                });
            var context = new Mock<IChannelHandlerContext>();
            context.Setup(i => i.WriteAsync(It.IsAny<object>()))
                .Callback<object>(i => result = i)
                .Returns(Task.CompletedTask);
            var handler = new DotNettyServerHandler(mockServerHandler.Object);
            Assert.True(handler.IsSharable);
            handler.ChannelRead(context.Object, new Request() { Version = 3 });
            Assert.NotNull(result);
            Assert.IsType<Response>(result);
            Assert.Equal(3, ((Response)result).Version);
        }
    }
}