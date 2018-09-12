using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace Tars.Net.Codecs
{
    public class ResponseDecoder : ByteToMessageDecoder
    {
        private readonly IDecoder<IByteBuffer> decoder;

        public ResponseDecoder(IDecoder<IByteBuffer> decoder)
        {
            this.decoder = decoder;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            while (input.IsReadable())
            {
                output.Add(decoder.DecodeResponse(input));
            }
        }
    }
}