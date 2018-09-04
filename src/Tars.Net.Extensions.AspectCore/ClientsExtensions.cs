using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using Tars.Net.Extensions.AspectCore;

namespace Tars.Net.Clients
{
    public static partial class AspectClientsExtensions
    {
        public const string Context_IsRpcClient = "#IsRpcClient";
        public const string Context_IngorePrefix = "#";

        public static IServiceCollection AddAop(this IServiceCollection services, Action<IAspectConfiguration> configure = null)
        {
            services.TryAddSingleton<IClientProxyCreator, AspectCoreClientProxyCreator>();
            services.TryAddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            services.TryAddSingleton<RpcClientInvokerFactory>();
            services.AddDynamicProxy(c =>
            {
                c.ValidationHandlers.Add(new RpcAspectValidationHandler());
                configure?.Invoke(c);
            });
            return services;
        }

        public static bool IsRpcClient(this AspectContext context)
        {
            return context.AdditionalData.ContainsKey(Context_IsRpcClient);
        }

        public static void SetContext(this IDictionary<string, string> context, IDictionary<string, object> additionalData)
        {
            foreach (var data in additionalData.Where(i=> !i.Key.StartsWith(Context_IngorePrefix)))
            {
                if (context.ContainsKey(data.Key))
                {
                    context[data.Key] = data.Value.ToString();
                }
                else
                {
                    context.Add(data.Key, data.Value.ToString());
                }
            }
        }

        public static void SetContext(this IDictionary<string, object> additionalData, IDictionary<string, string> context)
        {
            foreach (var data in context.Where(i => !i.Key.StartsWith(Context_IngorePrefix)))
            {
                if (additionalData.ContainsKey(data.Key))
                {
                    additionalData[data.Key] = data.Value.ToString();
                }
                else
                {
                    additionalData.Add(data.Key, data.Value.ToString());
                }
            }
        }
    }
}