using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Tars.Net.Clients;
using Tars.Net.Clients.Tcp;
using Tars.Net.Clients.Udp;
using Tars.Net.DotNetty.Hosting;
using Tars.Net.Hosting;
using Tars.Net.Hosting.Tcp;
using Tars.Net.Hosting.Udp;
using Tars.Net.UT.DotNetty;

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

        public static IServiceCollection AddUdpClient(this IServiceCollection services)
        {
            return services.AddSingleton<IRpcClient, UdpClient>();
        }

        public static IHostBuilder UseUdpHost(this IHostBuilder builder, params Assembly[] assemblies)
        {
            return builder.ConfigureServices((hostContext, services) =>
            {
                services.ReigsterRpcServices(assemblies);
                services.TryAddSingleton<DotNettyServerHandler>();
                services.AddHostedService<UdpServerHost>();
            });
        }


        public static IHostBuilder UseTarsHost(this IHostBuilder builder, params Assembly[] assemblies)
        {
          return  builder.ConfigureServices((hostContext, services) =>
            {
                services.ReigsterRpcServices(assemblies);
                services.TryAddSingleton<DotNettyServerHandler>();
                services.AddHostedService<LibuvTarsServerHost>();
            });
        }

    }
}