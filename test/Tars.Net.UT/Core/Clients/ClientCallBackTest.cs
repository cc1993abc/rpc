using System;
using Tars.Net.Clients;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Core.Clients
{
    public class ClientCallBackTest
    {
        private readonly ClientCallBack sut;

        public ClientCallBackTest()
        {
            sut = new ClientCallBack();
        }

        [Fact]
        public void NewCallBackIdShouldBeRight()
        {
            for (int i = 1; i < 10; i++)
            {
                Assert.Equal(i, sut.NewCallBackId());
            }
        }

        [Fact]
        public void NewCallBackTaskWhenTimeoutShouldBeThrowEx()
        {
            TarsException ex = null;
            try
            {
                sut.NewCallBackTask(1, 1, "test", "test").ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                ex = e as TarsException;
            }

            Assert.NotNull(ex);
            Assert.Equal(RpcStatusCode.AsyncCallTimeout, ex.RpcStatusCode);
            Assert.Equal("Call test.test timeout.", ex.Message);
        }

        [Fact]
        public void CallBackWhenResponseShouldBeThrowEx()
        {
            var task = sut.NewCallBackTask(2, 1, "test1", "test");
            var oldResp = new Response()
            {
                RequestId = 2
            };
            var (servantName, funcName) = sut.FindRpcMethod(2).Value;
            Assert.Equal("test1", servantName);
            Assert.Equal("test", funcName);
            Assert.False(sut.FindRpcMethod(232).HasValue);
            sut.CallBack(oldResp);
            var resp = task.ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Same(oldResp, resp);
        }
    }
}