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
        public void TestDecoDotNettyClientHandlerWhenChannelWritable()
        {
            var mockServerHandler = new Mock<IServerHandler>();
            mockServerHandler.Setup(i => i.ProcessAsync(It.IsAny<Request>()))
                .Returns<Request>(i =>
                {
                    return Task.FromResult(i.CreateResponse());
                });
            var context = new Mock<IChannelHandlerContext>();
            var channel = new Mock<IChannel>();
            context.SetupGet(i => i.Channel).Returns(channel.Object);
            channel.SetupGet(i => i.IsWritable).Returns(true);
            context.Setup(i => i.WriteAndFlushAsync(It.IsAny<object>()))
                .Callback<object>(result => 
                {
                    Assert.NotNull(result);
                    Assert.IsType<Response>(result);
                    Assert.Equal(3, ((Response)result).Version);
                })
                .Returns(Task.CompletedTask);
            var handler = new DotNettyServerHandler(mockServerHandler.Object);
            Assert.True(handler.IsSharable);
            handler.ChannelRead(context.Object, new Request() { Version = 3 });
        }

        [Fact]
        public void TestDecoDotNettyClientHandlerWhenChannelNotWritable()
        {
            object result = null;
            var mockServerHandler = new Mock<IServerHandler>();
            mockServerHandler.Setup(i => i.ProcessAsync(It.IsAny<Request>()))
                .Returns<Request>(i =>
                {
                    return Task.FromResult(i.CreateResponse());
                });
            var context = new Mock<IChannelHandlerContext>();
            var channel = new Mock<IChannel>();
            context.SetupGet(i => i.Channel).Returns(channel.Object);
            channel.SetupGet(i => i.IsWritable).Returns(false);
            context.Setup(i => i.WriteAndFlushAsync(It.IsAny<object>()))
                .Callback<object>(i => result = i)
                .Returns(Task.CompletedTask);
            var handler = new DotNettyServerHandler(mockServerHandler.Object);
            Assert.True(handler.IsSharable);
            handler.ChannelRead(context.Object, new Request() { Version = 3 });
            Assert.Null(result);
        }
    }
}