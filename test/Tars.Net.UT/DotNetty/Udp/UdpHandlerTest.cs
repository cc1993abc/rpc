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
    public class UdpHandlerTest
    {
        [Fact]
        public void TestExceptionCaught()
        {
            var mockLogger = new Mock<ILogger<UdpHandler>>();
            var context = new Mock<IChannelHandlerContext>();
            var handler = new UdpHandler(mockLogger.Object);
            handler.ChannelRead(context.Object, null);
            handler.ChannelRead(context.Object, new DatagramPacket(Unpooled.Empty, new IPEndPoint(IPAddress.Any, 88)));
            handler.ExceptionCaught(context.Object, new Exception("tEST"));
        }
    }
}