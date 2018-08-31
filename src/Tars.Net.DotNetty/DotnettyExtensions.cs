using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        public static IServerHostBuilder UseLibuvTcpHost(this IServerHostBuilder builder)
        {
            return builder.ConfigureServices(i =>
            {
                i.TryAddSingleton<DotNettyServerHandler>();
                i.TryAddSingleton<IServerHost, LibuvTcpServerHost>();
            });
        }
    }
}