using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public abstract class ResponseEncoder : MessageToMessageEncoder<Response>
    {
        protected override void Encode(IChannelHandlerContext context, Response message, List<object> output)
        {
            if (message == null)
            {
                return;
            }

            output.Add(EncodeResponse(message));
        }

        public abstract IByteBuffer EncodeResponse(Response message);
    }
}