using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net.Hosting;

namespace Tcp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new ServerHostBuilder()
                .ConfigureConfiguration(i => i.AddJsonFile("app.json"))
                .AddHostConfiguration()
                .ConfigureLog(i => i.AddConsole())
                .UseLibuvTcp()
                .ReigsterRpc()
                .Build();

            await host.RunAsync(async () =>
                {
                    var logger = host.Provider.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Press any key to stop.");
                    Console.ReadLine();
                });
        }
    }
}