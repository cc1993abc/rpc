using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public abstract class ResponseDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            while (input.IsReadable())
            {
                output.Add(DecodeResponse(input));
            }
        }

        public abstract Response DecodeResponse(IByteBuffer input);

        public abstract void DecodeResponseContent(Response resp);
    }
}