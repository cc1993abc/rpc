using DotNetty.Buffers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tars.Net.Hosting.Tcp;

namespace Tars.Net.Hosting
{
    public static class ServerHostExtensions
    {
        public static IServerHostBuilder UseLibuvTcpHost(this IServerHostBuilder builder)
        {
            return builder.ConfigureServices(i =>
            {
                i.TryAddSingleton<IServerInvoker, ServerInvoker>();
                i.TryAddSingleton<DotNettyServerHandler>();
                i.TryAddSingleton<IServerHost, LibuvTcpServerHost>();
            });
        }
    }
}