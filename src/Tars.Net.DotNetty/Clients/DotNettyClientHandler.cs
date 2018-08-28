using DotNetty.Transport.Channels;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class DotNettyClientHandler : SimpleChannelInboundHandler<Response>
    {
        private readonly IClientCallBack callBack;

        public override bool IsSharable => true;

        public DotNettyClientHandler(IClientCallBack callBack)
        {
            this.callBack = callBack;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Response msg)
        {
            callBack.CallBack(msg);
        }
    }
}