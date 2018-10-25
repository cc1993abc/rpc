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

        protected override async void ChannelRead0(IChannelHandlerContext ctx, Request msg)
        {
            var resp = await handler.ProcessAsync(msg);
            if (ctx.Channel.IsWritable)
            {
                await ctx.WriteAndFlushAsync(resp);
            }
        }
    }
}