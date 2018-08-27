using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public class ResponseEncoder : MessageToMessageEncoder<Response>
    {
        private readonly IEncoder encoder;

        public ResponseEncoder(IEncoder encoder)
        {
            this.encoder = encoder;
        }

        protected override void Encode(IChannelHandlerContext context, Response message, List<object> output)
        {
            if (message == null)
            {
                return;
            }

            output.Add(encoder.EncodeResponse(message));
        }
    }
}