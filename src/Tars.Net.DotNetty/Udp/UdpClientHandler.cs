using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;

namespace Tars.Net.Hosting.Udp
{
    public class UdpClientHandler : ChannelHandlerAdapter
    {
        private readonly ILogger<UdpClientHandler> logger;

        public UdpClientHandler(ILogger<UdpClientHandler> logger)
        {
            this.logger = logger;
        }

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
            logger.LogError(exception.Message, exception);
            context.CloseAsync();
        }
    }
}