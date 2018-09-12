using Tars.Net.Exceptions;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Abstractions.Metadata
{
    public class ResponseTest
    {
        [Fact]
        public void CheckResultStatusWhenErrorShouldThrowEx()
        {
            var resp = new Response()
            {
                ResultStatusCode = RpcStatusCode.ServerUnknownErr,
                ResultDesc = "Test"
            };

            var ex = Assert.Throws<TarsException>(() => resp.CheckResultStatus());
            Assert.Equal(RpcStatusCode.ServerUnknownErr, ex.RpcStatusCode);
            Assert.Equal("Test", ex.Message);
        }

        [Fact]
        public void CheckResultStatusWhenErrorShouldNoEx()
        {
            var resp = new Response()
            {
                ResultStatusCode = RpcStatusCode.ServerSuccess,
                ResultDesc = "Test"
            };

            resp.CheckResultStatus();
        }
    }
}