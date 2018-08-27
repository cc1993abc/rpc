using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Clients.Tcp;

namespace Tars.Net.Clients
{
    public static partial class ClientExtensions
    {
        public static IServiceCollection AddLibuvTcpClient(this IServiceCollection services)
        {
            return services.AddSingleton<IRpcClient, LibuvTcpClient>();
        }
    }
}