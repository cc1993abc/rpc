using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Tars.Net.Exceptions;
using Tars.Net.Hosting;
using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    [Rpc("Test")]
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
            return Task.FromResult<object>(3);
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
            return Task.FromResult<object>(3);
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

        public ServerInvokerTest()
        {
            var decoder = new Mock<IContentDecoder>();
            var services = new ServiceCollection();
            services.ReigsterRpcServices();
            services.AddSingleton<ServerInvoker>();
            services.AddSingleton(decoder.Object);
            sut = services.BuildServiceProvider()
                .GetRequiredService<ServerInvoker>();
        }

        [Fact]
        public void InvokeWhenNoServantShouldThrowEx()
        {
            var req = new Request()
            {
                ServantName = "O"
            };
            var resp = req.CreateResponse();
            var ex = Assert.Throws<TarsException>(() => sut.Invoke(req, resp));
            Assert.Equal(RpcStatusCode.ServerNoServantErr, ex.RpcStatusCode);
            Assert.Equal("No found servant, serviceName[O]", ex.Message);
        }

        [Fact]
        public void InvokeWhenNoFuncShouldThrowEx()
        {
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "Go"
            };
            var resp = req.CreateResponse();
            var ex = Assert.Throws<TarsException>(() => sut.Invoke(req, resp));
            Assert.Equal(RpcStatusCode.ServerNoFuncErr, ex.RpcStatusCode);
            Assert.Equal("No found methodInfo, serviceName[Test], methodName[Go]", ex.Message);
        }

        [Fact]
        public void InvokeWhenGetVShouldOutParameterBe3()
        {
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "GetV",
                Parameters = new object[] { 1 }
            };
            var resp = req.CreateResponse();
            sut.Invoke(req, resp);
            Assert.Equal(3, req.Parameters[0]);
            Assert.Equal(3, ((Task<object>)resp.ReturnValue).Result);
            Assert.Single(resp.ReturnParameters);
            Assert.Equal(3, resp.ReturnParameters[0]);
        }

        [Fact]
        public void InvokeWhenGetOnewayShouldOutParameterBe3()
        {
            var req = new Request()
            {
                ServantName = "Test",
                FuncName = "GetOneway",
                Parameters = new object[] { 1 }
            };
            var resp = req.CreateResponse();
            sut.Invoke(req, resp);
            Assert.Equal(3, req.Parameters[0]);
            Assert.Equal(3, ((Task<object>)resp.ReturnValue).Result);
            Assert.Null(resp.ReturnParameters);
        }
    }
}
