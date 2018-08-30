using AspectCore.Configuration;
using System;
using Tars.Net.Clients;
using Tars.Net.Extensions.AspectCore;

namespace Tars.Net.Hosting
{
    public static partial class ServerHostBuilderExtensions
    {
        public static IServerHostBuilder UseAop(this IServerHostBuilder builder, Action<IAspectConfiguration> configure = null)
        {
            builder.Services.AddAop(configure);
            return new AopServerHostBuilder(builder);
        }
    }
}