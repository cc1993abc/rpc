using DotNetty.Buffers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tars.Net.Codecs;
using Tars.Net.Extensions.AspectCore;
using TcpCommon;
using Tars.Net;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;

namespace TcpServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new AspectCoreServerHostBuilder()
                .ConfigureServices(i =>
                {
                    //todo: add Decoder and Encoder
                    i.TryAddSingleton<IDecoder<IByteBuffer>, TestDecoder>();
                    i.TryAddSingleton<IEncoder<IByteBuffer>, TestEncoder>();
                    i.TryAddSingleton<IContentDecoder, TestContentDecoder>();
                    i.AddLibuvTcpClient();
                    i.AddConfiguration();
                })
                .ReigsterRpcClients()
                .ReigsterRpcServices()
                .ConfigureConfiguration(i => i.AddJsonFile("app.json"))
                .ConfigureLog(i => i.AddConsole())
                .UseLibuvTcpHost()
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