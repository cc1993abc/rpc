using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net;
using TcpCommon;

namespace TcpClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            var service = new ServiceCollection()
                .AddSingleton<RequestDecoder, TestRequestDecoder>()
                .AddSingleton<RequestEncoder, TestRequestEncoder>()
                .AddSingleton<ResponseDecoder, TestResponseDecoder>()
                .AddSingleton<ResponseEncoder, TestResponseEncoder>()
                .AddSingleton(i => builder.AddJsonFile("app.json").Build())
                .AddLogging(j => j.AddConsole())
                .AddLibuvTcpClient()
                .ReigsterRpcClients()
                .BuildAspectCoreServiceProvider();

            var rpc = service.GetRequiredService<IHelloRpc>();
            var result = await rpc.HelloTask(3, "Vic");
            Console.WriteLine(result);
        }
    }
}