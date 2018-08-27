using DotNetty.Transport.Channels;
using System;

namespace Tars.Net.Hosting.Tcp
{
    public class TcpHandler : ChannelHandlerAdapter
    {
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}