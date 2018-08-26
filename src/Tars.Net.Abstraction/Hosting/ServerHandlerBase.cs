using DotNetty.Transport.Channels;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public abstract class ServerHandlerBase : SimpleChannelInboundHandler<Request>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, Request msg)
        {
            ctx.WriteAsync(Process(msg));
        }

        public abstract Response Process(Request msg);
    }
}