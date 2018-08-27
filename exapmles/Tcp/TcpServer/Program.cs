using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Hosting;
using TcpCommon;

namespace TcpServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new ServerHostBuilder()
                .ConfigureServices(i =>
                {
                    //todo: add Decoder and Encoder
                    i.TryAddSingleton<RequestDecoder, TestRequestDecoder>();
                    i.TryAddSingleton<RequestEncoder, TestRequestEncoder>();
                    i.TryAddSingleton<ResponseDecoder, TestResponseDecoder>();
                    i.TryAddSingleton<ResponseEncoder, TestResponseEncoder>();
                    i.AddLibuvTcpClient();
                    i.ReigsterRpcClients();
                    i.ReigsterRpcServices();
                    i.AddConfiguration();
                })
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