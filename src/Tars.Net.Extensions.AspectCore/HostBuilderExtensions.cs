using AspectCore.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Tars.Net.Hosting
{
    public static partial class HostBuilderExtensions
    {
        public static IHostBuilder UseAop(this IHostBuilder builder, Action<IAspectConfiguration> configure = null)
        {
            return builder.UseServiceProviderFactory(new AopServiceProviderFactory(configure));
        }
    }
}