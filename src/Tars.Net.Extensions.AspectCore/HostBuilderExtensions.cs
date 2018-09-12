using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Tars.Net.Extensions.AspectCore.DynamicProxy;

namespace Tars.Net.Hosting
{
    public static partial class HostBuilderExtensions
    {
        public static IHostBuilder UseAop(this IHostBuilder builder, Action<IAspectConfiguration> configure = null)
        {
            builder.ConfigureServices((c, i) => i.AddSingleton<IAspectActivatorFactory, OriginExceptionAspectActivatorFactory>());
            return builder.UseServiceProviderFactory(new AopServiceProviderFactory(configure));
        }
    }
}