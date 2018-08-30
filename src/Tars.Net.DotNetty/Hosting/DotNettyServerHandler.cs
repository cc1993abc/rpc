using DotNetty.Transport.Channels;
using Tars.Net.Hosting;
using Tars.Net.Metadata;

namespace Tars.Net.DotNetty.Hosting
{
    public class DotNettyServerHandler : SimpleChannelInboundHandler<Request>
    {
        public override bool IsSharable => true;

        private readonly IServerHandler handler;

        public DotNettyServerHandler(IServerHandler handler)
        {
            this.handler = handler;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Request msg)
        {
            ctx.WriteAsync(handler.Process(msg));
        }
    }
}