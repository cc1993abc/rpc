using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Moq;
using Tars.Net.Codecs;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.DotNetty.Codecs
{
    public class ResponseDecoderTest
    {
        [Fact]
        public void TestDecodeResponse()
        {
            object result = null;
            var mockDecoder = new Mock<IDecoder<IByteBuffer>>();
            mockDecoder.Setup(i => i.DecodeResponse(It.IsAny<IByteBuffer>()))
                .Returns<IByteBuffer>(i =>
                {
                    i.ReadBytes(1);
                    i.MarkReaderIndex();
                    return new Response();
                });
            var context = new Mock<IChannelHandlerContext>();
            context.Setup(i => i.FireChannelRead(It.IsAny<object>()))
                .Callback<object>(i => result = i);
            var reqDecoder = new ResponseDecoder(mockDecoder.Object);
            reqDecoder.ChannelRead(context.Object, Unpooled.WrappedBuffer(new byte[] { 3 }));
            Assert.NotNull(result);
            Assert.IsType<Response>(result);
        }
    }
}