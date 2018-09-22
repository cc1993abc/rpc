using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using Tars.Net.Hosting.Udp;
using Xunit;

namespace Tars.Net.UT.DotNetty.Udp
{
    public class UdpClientHandlerTest
    {
        [Fact]
        public void TestExceptionCaught()
        {
            var mockLogger = new Mock<ILogger<UdpClientHandler>>();
            var context = new Mock<IChannelHandlerContext>();
            var handler = new UdpClientHandler(mockLogger.Object);
            handler.ChannelRead(context.Object, null);
            handler.WriteAsync(context.Object, Unpooled.Empty);
            handler.ChannelRead(context.Object, new DatagramPacket(Unpooled.Empty, new IPEndPoint(IPAddress.Any, 88)));
            handler.ExceptionCaught(context.Object, new Exception("tEST"));
        }
    }
}