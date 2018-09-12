using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Moq;
using Tars.Net.Codecs;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.DotNetty.Codecs
{
    public class ResponseEncoderTest
    {
        [Fact]
        public void TestEncodeResponse()
        {
            object result = null;
            var mockDecoder = new Mock<IEncoder<IByteBuffer>>();
            mockDecoder.Setup(i => i.EncodeResponse(It.IsAny<Response>()))
                .Returns<Response>(i =>
                {
                    var buffer = Unpooled.Buffer(1);
                    buffer.WriteBoolean(true);
                    buffer.MarkWriterIndex();
                    return buffer;
                });
            var context = new Mock<IChannelHandlerContext>();
            context.Setup(i => i.WriteAsync(It.IsAny<object>()))
                .Callback<object>(i => result = i);
            var reqDecoder = new ResponseEncoder(mockDecoder.Object);
            reqDecoder.WriteAsync(context.Object, new Response());
            Assert.NotNull(result);
            Assert.True(((IByteBuffer)result).ReadBoolean());
            result = null;
            reqDecoder.WriteAsync(context.Object, null);
            Assert.Null(result);
        }
    }
}