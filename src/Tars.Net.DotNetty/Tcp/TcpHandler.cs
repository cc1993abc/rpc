using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;

namespace Tars.Net.Hosting.Tcp
{
    public class TcpHandler : ChannelHandlerAdapter
    {
        private readonly ILogger<TcpHandler> logger;

        public TcpHandler(ILogger<TcpHandler> logger)
        {
            this.logger = logger;
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            logger.LogError(exception, exception.Message);
            context.CloseAsync();
        }
    }
}