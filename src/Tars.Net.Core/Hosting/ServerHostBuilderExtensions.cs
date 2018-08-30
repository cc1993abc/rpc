using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Tars.Net.Clients;
using Tars.Net.Hosting;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public static class ServerHostBuilderExtensions
    {
        public static IServerHostBuilder ReigsterRpcServices(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            var rpcMetadata = builder.Services.GetRpcMetadata();
            foreach (var (service, implementation) in rpcMetadata.RpcServices)
            {
                builder.Services.TryAddSingleton(service.GetReflector().GetMemberInfo().AsType(), implementation.GetReflector().GetMemberInfo().AsType());
            }

            builder.Services.TryAddSingleton<IServerInvoker, ServerInvoker>();
            builder.Services.TryAddSingleton<IServerHandler, ServerHandler>();
            return builder;
        }

        public static IServerHostBuilder ConfigureLog(this IServerHostBuilder builder, Action<ILoggingBuilder> configure)
        {
            return builder.ConfigureServices(i => i.AddLogging(configure));
        }

        public static IServerHostBuilder ReigsterRpcClients(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            builder.Services.ReigsterRpcClients();
            return builder;
        }
    }
}