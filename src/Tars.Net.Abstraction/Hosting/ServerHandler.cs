using DotNetty.Transport.Channels;
using Tars.Net.Codecs;

namespace Tars.Net.Hosting
{
    public abstract class ServerHandler : SimpleChannelInboundHandler<Request>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, Request msg)
        {
            ctx.WriteAsync(Process(msg));
        }

        public abstract Response Process(Request msg);
    }
}