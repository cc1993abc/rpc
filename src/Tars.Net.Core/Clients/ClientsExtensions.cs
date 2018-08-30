using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Clients
{
    public static partial class ClientsExtensions
    {
        public static IServiceCollection AddTarsClient(this IServiceCollection serivces, Action<ITarsBuilder> option)
        {
            var builder = new TarsBuilder(serivces);
            builder.ReigsterRpcClients();

            option(builder);

            return builder.Services;
        }
    }
}