using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Core.Clients
{
    public class RpcClientFactoryTest
    {
        private readonly RpcClientFactory sut;

        public RpcClientFactoryTest()
        {
            var rpcClient = new Mock<IRpcClient>();
            rpcClient.SetupGet(i => i.Protocol).Returns(RpcProtocol.Tcp);
            var clients = new IRpcClient[] { rpcClient.Object };
            var config = new RpcConfiguration()
            {
                ClientConfig = new Dictionary<string, ClientConfiguration>()
                 {
                      {"NoUdp", new ClientConfiguration() { Protocol = RpcProtocol.Udp } },
                      {"Tcp", new ClientConfiguration() { Protocol = RpcProtocol.Tcp, Host = "127.0.0.1", Port = 3333, Timeout = 30 } }
                 }
            };
            var calback = new Mock<IClientCallBack>();
            calback.Setup(i => i.NewCallBackTask(0, 30, "Tcp", "ReturnParametersNull"))
                .ReturnsAsync(new Response()
                {
                    RequestId = 2,
                    Timeout = 2,
                    ReturnParameters = null
                });
            sut = new RpcClientFactory(clients, config, calback.Object);
        }

        [Fact]
        public async Task SendAsyncWhenNoServantNameConfigShouldThrowEx()
        {
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.SendRequestAsync(new Request()
            {
                ServantName = "Test"
            }));

            Assert.Equal("No find Rpc client config for Test", ex.Message);
        }

        [Fact]
        public async Task SendAsyncWhenNoProtocolShouldThrowEx()
        {
            var ex = await Assert.ThrowsAsync<NotSupportedException>(() => sut.SendRequestAsync(new Request()
            {
                ServantName = "NoUdp"
            }));

            Assert.Equal("No find Rpc client which supported Udp", ex.Message);
        }

        [Fact]
        public async Task SendAsyncWhenHasConfigShouldNoEx()
        {
            await sut.SendRequestAsync(new Request()
            {
                ServantName = "Tcp"
            });
        }

        [Fact]
        public async Task SendAsyncWhenOnewayShouldDefaultResponse()
        {
            var resp = await sut.SendAsync(new Request()
            {
                ServantName = "Tcp",
                IsOneway = true
            });
            Assert.Equal(RpcStatusCode.ServerSuccess, resp.ResultStatusCode);
        }

        [Fact]
        public async Task SendAsyncWhenReturnParametersNullShouldNoHandleReturnParameters()
        {
            var resp = await sut.SendAsync(new Request()
            {
                ServantName = "Tcp",
                FuncName = "ReturnParametersNull",
                Codec = Codec.Tars
            });
            Assert.Equal(RpcStatusCode.ServerSuccess, resp.ResultStatusCode);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(2, resp.Timeout);
            Assert.Equal(2, resp.RequestId);
            Assert.Null(resp.ReturnParameters);
        }
    }
}