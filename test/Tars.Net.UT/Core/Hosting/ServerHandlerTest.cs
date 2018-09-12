using Moq;
using System;
using Tars.Net.Codecs;
using Tars.Net.Exceptions;
using Tars.Net.Hosting;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    public class ServerHandlerTest
    {
        private readonly ServerHandler sut;

        public ServerHandlerTest()
        {
            var invoker = new Mock<IServerInvoker>();
            invoker.Setup(i => i.Invoke(It.Is<Request>(j => j.FuncName == "ThrowTarsException"), It.IsAny<Response>()))
                .Throws(new TarsException(RpcStatusCode.ServerNoFuncErr, "Test"));
            invoker.Setup(i => i.Invoke(It.Is<Request>(j => j.FuncName == "ThrowException"), It.IsAny<Response>()))
                .Throws(new Exception("ThrowException"));
            sut = new ServerHandler(invoker.Object);
        }

        [Fact]
        public void ProcessWhenPingShouldBeDefaultRseponse()
        {
            var req = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "tars_ping",
                Timeout = 33
            };
            var resp = sut.Process(req);
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerSuccess, resp.ResultStatusCode);
        }

        [Fact]
        public void ProcessWhenThrowTarsExceptionShouldGetRpcStatusCode()
        {
            var req = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "ThrowTarsException",
                Timeout = 33
            };
            var resp = sut.Process(req);
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, resp.ResultStatusCode);
            Assert.Equal("Test", resp.ResultDesc);
        }

        [Fact]
        public void ProcessWhenThrowExceptionShouldGetServerUnknownErr()
        {
            var req = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "ThrowException",
                Timeout = 33
            };
            var resp = sut.Process(req);
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerUnknownErr, resp.ResultStatusCode);
            Assert.Equal("ThrowException", resp.ResultDesc);
        }
    }
}