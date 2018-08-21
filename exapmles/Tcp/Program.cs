using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tars.Csharp.Hosting;

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