using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Moq;
using System.Threading.Tasks;
using Tars.Net.DotNetty.Udp;
using Xunit;

namespace Tars.Net.UT.DotNetty.Udp
{
    public class UdpLengthFieldPrependerTest
    {
        [Fact]
        public async Task UdpLengthFieldPrependerWhenLength4ShouldBeAdd()
        {
            var buffer = Unpooled.Buffer(248)
                .WriteBoolean(false)
                .WriteInt(3);
            IByteBuffer result = null;
            var context = new Mock<IChannelHandlerContext>();
            context.SetupGet(i => i.Allocator).Returns(buffer.Allocator);
            context.Setup(i => i.WriteAsync(It.IsAny<object>()))
                .Callback<object>(i => result = (IByteBuffer)i)
                .Returns(Task.CompletedTask);
            var prepender = new UdpLengthFieldPrepender(4);
            await prepender.WriteAsync(context.Object, buffer);
            Assert.NotNull(result);
            Assert.Equal(9, result.ReadableBytes);
            Assert.Equal(5, result.ReadInt());
            Assert.True(prepender.IsSharable);
        }
    }
}