using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Exceptions;
using Tars.Net.Hosting;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    [Rpc]
    public interface ITestServer
    {
        Task<object> GetV(out int p);

        [Oneway]
        Task<object> GetOneway(out int p);

        void GetVNoP(out int p);

        void GetVNoP();

        void GetVNoPddd(out int p);

        void GetTarsEx();

    }

    public class TestServer : ITestServer
    {
        public Task<object> GetV(out int p)
        {
            p = 3;
            return Task.FromResult<object>(3);
        }

        public Task<object> GetOneway(out int p)
        {
            p = 3;
            return Task.FromResult<object>(4);
        }

        public void GetVNoP(out int p)
        {
            throw new NotImplementedException();
        }

        public void GetVNoP()
        {
            // Method intentionally left empty.
        }

        public void GetVNoPddd(out int p)
        {
            throw new NotImplementedException();
        }

        public void GetTarsEx()
        {
            throw new TarsException(RpcStatusCode.ServerEncodeErr, "Test");
        }
    }

    public class TestServerOne : ITestServer
    {
        public Task<object> GetV(out int p)
        {
            p = 3;
            return Task.FromResult<object>(3);
        }

        public Task<object> GetOneway(out int p)
        {
            p = 3;
            return Task.FromResult<object>(4);
        }

        public void GetVNoP(out int p)
        {
            throw new NotImplementedException();
        }

        public void GetVNoP()
        {
            // Method intentionally left empty.
        }

        public void GetVNoPddd(out int p)
        {
            throw new NotImplementedException();
        }

        public void GetTarsEx()
        {
            throw new TarsException(RpcStatusCode.ServerEncodeErr, "Test");
        }
    }

    public class ServerInvokerTest
    {
        private readonly ServerInvoker sut;
        private readonly IRpcMetadata rpcMetadata;

        public ServerInvokerTest()
        {
            var services = new ServiceCollection();
            services.ReigsterRpcServices();
            services.AddSingleton<ServerInvoker>();
            rpcMetadata = services.GetRpcMetadata();
            services.AddSingleton(new RpcConfiguration()
            {
                ServiceConfig = new List<ServiceConfiguration>()
                 {
                    new ServiceConfiguration()
                    {
                        Interface = "Tars.Net.UT.Core.Hosting.ITestServer",
                        Servant = "Test",
                        CodecVersion = 3
                    }
                }
            });
            var provider = services.BuildServiceProvider();
            rpcMetadata.Init(provider);
            sut = provider.GetRequiredService<ServerInvoker>();
        }

        [Fact]
        public async Task InvokeWhenNoServantShouldThrowEx()
        {
            var req = new Request()
            {
                ServantName = "O"
            };
            var resp = req.CreateResponse();
            resp = await sut.InvokeAsync(req, resp);
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, resp.ResultStatusCode);
            Assert.Equal("No found methodInfo, serviceName[O], methodName[]", resp.ResultDesc);
        }

        [Fact]
        public async Task InvokeWhenNoFuncShouldThrowEx()
        {
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "Go"
            };
            var resp = req.CreateResponse();
            resp = await sut.InvokeAsync(req, resp);
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, resp.ResultStatusCode);
            Assert.Equal("No found methodInfo, serviceName[Test], methodName[Go]", resp.ResultDesc);
        }

        [Fact]
        public async Task InvokeWhenGetVShouldOutParameterBe3()
        {
            var (methodInfo, isOneway, outParameters, codec, version, serviceType) = rpcMetadata.FindRpcMethod("Test", "GetV");
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "GetV",
                Parameters = new object[] { 1 },
                Mehtod = methodInfo,
                ServiceType = serviceType,
                ReturnParameterTypes = outParameters,
                IsOneway = isOneway
            };
            var resp = req.CreateResponse();
            await sut.InvokeAsync(req, resp);
            Assert.Equal(3, req.Parameters[0]);
            Assert.Equal(3, ((Task<object>)resp.ReturnValue).Result);
            Assert.Single(resp.ReturnParameters);
            Assert.Equal(3, resp.ReturnParameters[0]);
        }

        [Fact]
        public async Task InvokeWhenGetOnewayShouldOutParameterBe3()
        {
            var (methodInfo, isOneway, outParameters, codec, version, serviceType) = rpcMetadata.FindRpcMethod("Test", "GetOneway");
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "GetOneway",
                Parameters = new object[] { 1 },
                Mehtod = methodInfo,
                ServiceType = serviceType,
                ReturnParameterTypes = outParameters,
                IsOneway = isOneway
            };
            var resp = req.CreateResponse();
            await sut.InvokeAsync(req, resp);
            Assert.Equal(3, req.Parameters[0]);
            Assert.Equal(4, ((Task<object>)resp.ReturnValue).Result);
            Assert.Single(resp.ReturnParameters);
            Assert.Null(resp.ReturnParameters[0]);
        }

        [Fact]
        public async Task ProcessWhenThrowTarsExceptionShouldGetRpcStatusCode()
        {
            var req = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "GetTarsEx",
                Timeout = 33,
                Mehtod = typeof(TestServer).GetMethod("GetTarsEx"),
                ServiceType = typeof(ITestServer)
            };
            var resp = await sut.InvokeAsync(req, req.CreateResponse());
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerEncodeErr, resp.ResultStatusCode);
            Assert.Equal("Test", resp.ResultDesc);
        }

        [Fact]
        public async Task ProcessWhenThrowExceptionShouldGetServerUnknownErr()
        {
            var req = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "ThrowException",
                Timeout = 33,
                Mehtod = typeof(TestServerOne).GetMethod("GetVNoPddd")
            };
            var resp = await sut.InvokeAsync(req, req.CreateResponse());
            Assert.Equal(req.Version, resp.Version);
            Assert.Equal(req.MessageType, resp.MessageType);
            Assert.Equal(req.RequestId, resp.RequestId);
            Assert.Equal(req.ServantName, resp.ServantName);
            Assert.Equal(req.FuncName, resp.FuncName);
            Assert.Equal(req.Timeout, resp.Timeout);
            Assert.Equal(Codec.Tars, resp.Codec);
            Assert.Equal(RpcStatusCode.ServerUnknownErr, resp.ResultStatusCode);
            Assert.StartsWith("Value cannot be null.",resp.ResultDesc);
        }
    }
}