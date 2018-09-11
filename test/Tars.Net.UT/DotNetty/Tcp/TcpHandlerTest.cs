using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Tars.Net.Hosting.Tcp;
using Xunit;

namespace Tars.Net.UT.DotNetty.Tcp
{
    public class TcpHandlerTest
    {
        [Fact]
        public void TestExceptionCaught()
        {
            var mockLogger = new Mock<ILogger<TcpHandler>>();
            var context = new Mock<IChannelHandlerContext>();
            var handler = new TcpHandler(mockLogger.Object);
            handler.ChannelReadComplete(context.Object);
            handler.ExceptionCaught(context.Object, new Exception("tEST"));
        }
    }
}