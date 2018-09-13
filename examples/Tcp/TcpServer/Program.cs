using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Tars.Net.DotNetty;
using Tars.Net.Hosting;

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
                .ConfigureHostConfiguration(i => i.AddJsonFile("app.json"))
                .ConfigureLogging((hostContext, configLogging) =>
                 {
                     configLogging.AddConsole();
                 })
                .UseStartup<Startup>()
                //.UseLibuvTcpHost()
                .UseUdpHost()
                .UseAop()
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}