using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Tars.Net.Clients;
using Tars.Net.Clients.Tcp;
using Tars.Net.DotNetty.Hosting;
using Tars.Net.Hosting;
using Tars.Net.Hosting.Tcp;

namespace Tars.Net.DotNetty
{
    public static class DotnettyExtensions
    {
        public static IServiceCollection AddLibuvTcpClient(this IServiceCollection services)
        {
            return services.AddSingleton<IRpcClient, LibuvTcpClient>();
        }

        public static IHostBuilder UseLibuvTcpHost(this IHostBuilder builder, params Assembly[] assemblies)
        {
            return builder.ConfigureServices((hostContext, services) =>
                 {
                     services.ReigsterRpcServices(assemblies);
                     services.TryAddSingleton<DotNettyServerHandler>();
                     services.AddHostedService<LibuvTcpServerHost>();
                 });
        }
    }
}