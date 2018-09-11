using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class ResponseEncoder : MessageToMessageEncoder<Response>
    {
        private readonly IEncoder<IByteBuffer> encoder;

        public ResponseEncoder(IEncoder<IByteBuffer> encoder)
        {
            this.encoder = encoder;
        }

        protected override void Encode(IChannelHandlerContext context, Response message, List<object> output)
        {
            output.Add(encoder.EncodeResponse(message));
        }
    }
}