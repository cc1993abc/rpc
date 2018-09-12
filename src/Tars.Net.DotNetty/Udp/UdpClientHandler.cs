using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;

namespace Tars.Net.Hosting.Udp
{
    public class UdpClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is DatagramPacket packet)
            {
                context.FireChannelRead(packet.Content);
            }
            else
            {
                ReferenceCountUtil.Release(message);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}