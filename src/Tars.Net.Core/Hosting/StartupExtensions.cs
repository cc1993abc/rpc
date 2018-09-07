using Microsoft.Extensions.Hosting;
using System;

namespace Tars.Net.Hosting
{
    public static class StartupExtensions
    {
        public static IHostBuilder UseStartup<T>(this IHostBuilder builder) where T : IStartup
        {
            return builder.ConfigureServices((c, i) =>
            {
                Activator.CreateInstance<T>().ConfigureServices(i);
            });
        }
    }
}