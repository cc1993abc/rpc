using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public class RequestDecoder : ByteToMessageDecoder
    {
        private readonly IDecoder<IByteBuffer> decoder;

        public RequestDecoder(IDecoder<IByteBuffer> decoder)
        {
            this.decoder = decoder;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            output.Add(decoder.DecodeRequest(input));
        }
    }
}