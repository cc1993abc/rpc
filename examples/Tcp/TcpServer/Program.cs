using DotNetty.Buffers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
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
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    //todo: add Decoder and Encoder
                    services.TryAddSingleton<IDecoder<IByteBuffer>, TestDecoder>();
                    services.TryAddSingleton<IEncoder<IByteBuffer>, TestEncoder>();
                    services.TryAddSingleton<IContentDecoder, TestContentDecoder>();
                    services.AddConfiguration();
                })
                .ConfigureHostConfiguration(i => i.AddJsonFile("app.json"))
                .ConfigureLogging((hostContext, configLogging) =>
                 {
                     configLogging.AddConsole();
                 })
                .UseLibuvTcpHost()
                .UseAop()
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}