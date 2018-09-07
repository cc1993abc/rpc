using System;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Abstractions.Exceptions
{
    public class TarsExceptionTest
    {
        [Fact]
        public void WhenSetInnerExShouldBeRight()
        {
            var ex = new TarsException(RpcStatusCode.ProxyConnectErr, new Exception("Test"));
            Assert.Equal(RpcStatusCode.ProxyConnectErr, ex.RpcStatusCode);
            Assert.Equal("Test", ex.Message);
            Assert.Equal("Test", ex.InnerException.Message);
        }

        [Fact]
        public void WhenSetExMessageShouldBeRight()
        {
            var ex = new TarsException(RpcStatusCode.ProxyConnectErr, "Test");
            Assert.Equal(RpcStatusCode.ProxyConnectErr, ex.RpcStatusCode);
            Assert.Equal("Test", ex.Message);
            Assert.Null(ex.InnerException);
        }
    }
}