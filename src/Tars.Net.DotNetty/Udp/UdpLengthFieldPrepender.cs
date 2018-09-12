using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Tars.Net.DotNetty.Udp
{
    public class UdpLengthFieldPrepender : MessageToMessageEncoder<IByteBuffer>
    {
        public override bool IsSharable => true;

        private readonly int lengthFieldLength;

        public UdpLengthFieldPrepender(int lengthFieldLength)
        {
            this.lengthFieldLength = lengthFieldLength;
        }

        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            int length = message.ReadableBytes;
            var buffer = context.Allocator.Buffer(lengthFieldLength).WriteInt((short)length).WriteBytes(message);
            output.Add(buffer);
        }
    }
}