using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Net.Hosting.Udp
{
    public class UdpHandler : ChannelHandlerAdapter
    {
        private readonly ILogger<UdpHandler> logger;

        private EndPoint endPoint;
        private EndPoint sender;

        public UdpHandler(ILogger<UdpHandler> logger)
        {
            this.logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is DatagramPacket packet)
            {
                endPoint = packet.Sender;
                sender = packet.Recipient;
                context.FireChannelRead(packet.Content);
            }
            else
            {
                ReferenceCountUtil.Release(message);
            }
        }

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            return base.WriteAsync(context, new DatagramPacket(message as IByteBuffer, sender, endPoint));
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            logger.LogError(exception, exception.Message);
            context.CloseAsync();
        }
    }
}