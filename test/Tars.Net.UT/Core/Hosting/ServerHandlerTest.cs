using Moq;
using System;
using System.Threading.Tasks;
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
            invoker.Setup(i => i.InvokeAsync(It.Is<Request>(j => j.FuncName == "ThrowTarsException"), It.IsAny<Response>()))
                .Throws(new TarsException(RpcStatusCode.ServerNoFuncErr, "Test"));
            invoker.Setup(i => i.InvokeAsync(It.Is<Request>(j => j.FuncName == "ThrowException"), It.IsAny<Response>()))
                .Throws(new Exception("ThrowException"));
            sut = new ServerHandler(invoker.Object, new Mock<IRpcMetadata>().Object, new Mock<IServiceProvider>().Object);
        }

        [Fact]
        public async Task ProcessWhenPingShouldBeDefaultRseponse()
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
            var resp = await sut.ProcessAsync(req);
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerSuccess, resp.ResultStatusCode);
        }

        
    }
}