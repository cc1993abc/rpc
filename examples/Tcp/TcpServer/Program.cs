using DotNetty.Buffers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;
using Tars.Net.Hosting;
using TcpCommon;

namespace TcpServer
{
    public class Program
    {
        protected Program()
        {
        }

        private static async Task Main(string[] args)
        {
            var host = new ServerHostBuilder()
                .ConfigureServices(i =>
                {
                    //todo: add Decoder and Encoder
                    i.TryAddSingleton<IDecoder<IByteBuffer>, TestDecoder>();
                    i.TryAddSingleton<IEncoder<IByteBuffer>, TestEncoder>();
                    i.TryAddSingleton<IContentDecoder, TestContentDecoder>();
                    i.AddConfiguration();
                })
                .ReigsterRpcServices()
                .ConfigureConfiguration(i => i.AddJsonFile("app.json"))
                .ConfigureLog(i => i.AddConsole())
                .UseLibuvTcpHost()
                .UseAop()
                .Build();

            await host.RunAsync(() => Task.Run(() =>
                {
                    var logger = host.Provider.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Press any key to stop.");
                    Console.ReadLine();
                }));
        }
    }
}