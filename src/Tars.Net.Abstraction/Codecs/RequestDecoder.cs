using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public abstract class RequestDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            while (input.IsReadable())
            {
                output.Add(DecodeRequest(input));
            }
        }

        public abstract Request DecodeRequest(IByteBuffer input);

        public abstract void DecodeRequestContent(Request req);
    }
}