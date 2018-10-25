using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tars.Net.Attributes;
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
            var ex = await Assert.ThrowsAsync<TarsException>(() => sut.InvokeAsync(req, resp));
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, ex.RpcStatusCode);
            Assert.Equal("No found methodInfo, serviceName[O], methodName[]", ex.Message);
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
            var ex = await Assert.ThrowsAsync<TarsException>(() => sut.InvokeAsync(req, resp));
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, ex.RpcStatusCode);
            Assert.Equal("No found methodInfo, serviceName[Test], methodName[Go]", ex.Message);
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
    }
}