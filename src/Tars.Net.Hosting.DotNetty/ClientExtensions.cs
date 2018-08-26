using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Clients;
using Tars.Net.Clients.Tcp;

namespace Tars.Net.Hosting
{
    public static class ClientExtensions
    {
        public static IServiceCollection AddLibuvTcpClient(this IServiceCollection services)
        {
            return services.AddSingleton<IRpcClient, LibuvTcpClient>();
        }
    }
}