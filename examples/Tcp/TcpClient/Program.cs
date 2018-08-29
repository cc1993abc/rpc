using AspectCore.Extensions.DependencyInjection;
using DotNetty.Buffers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using TcpCommon;

namespace TcpClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                var service = new ServiceCollection()
                    .AddSingleton<IDecoder<IByteBuffer>, TestDecoder>()
                    .AddSingleton<IEncoder<IByteBuffer>, TestEncoder>()
                    .AddSingleton<IConfiguration>(i => builder.AddJsonFile("app.json").Build())
                    .AddLogging(j => j.AddConsole())
                    .AddConfiguration()
                    .AddLibuvTcpClient()
                    .ReigsterRpcClients()
                    .BuildAspectCoreServiceProvider();

                var rpc = service.GetRequiredService<IHelloRpc>();
                var result = rpc.Hello(3, "Vic");
                Console.WriteLine(result);
                result = rpc.Hello(3, "Victor");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //Console.ReadLine();
        }
    }
}