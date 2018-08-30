using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Clients
{
    public static partial class ClientsExtensions
    {
        public static IServiceCollection AddTarsClient(this IServiceCollection serivces, Action<ITarsClientBuilder> option)
        {
            var builder = new TarsClientBuilder(serivces);
            builder.ReigsterRpcClients();

            option(builder);

            return builder.Services;
        }
    }
}