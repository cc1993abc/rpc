using Microsoft.Extensions.DependencyInjection;
using Tars.Net.Attributes;
using Tars.Net.Clients.Tcp;
using Tars.Net.Hosting;

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