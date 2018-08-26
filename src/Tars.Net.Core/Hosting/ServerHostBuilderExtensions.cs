using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Clients;
using Tars.Net.Clients.Proxy;
using Tars.Net.Codecs;

namespace Tars.Net.Hosting
{
    public static class ServerHostBuilderExtensions
    {
        private static void ReigsterRpcDep(IServiceCollection services)
        {
            services.TryAddSingleton<IClientProxyCreater, ClientProxyCreater>();
            services.TryAddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            services.TryAddSingleton<IRpcClientFactory, RpcClientFactory>();
            services.TryAddSingleton<ServerHandlerBase, ServerHandler>();
            //todo: add Decoder and Encoder
            //services.TryAddSingleton<RequestDecoder, >();
            //services.TryAddSingleton<ResponseEncoder, >();
        }

        public static IServerHostBuilder ReigsterRpc(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            var all = GetAllHasAttributeTypes<RpcAttribute>();
            var (rpcServices, rpcClients) = GetAllRpcServicesAndClients(all);
            return builder.ConfigureServices(i =>
            {
                ReigsterRpcDep(i);
                foreach (var (service, implementation) in rpcServices)
                {
                    i.TryAddSingleton(service.GetReflector().GetMemberInfo().AsType(), implementation.GetReflector().GetMemberInfo().AsType());
                }

                foreach (var client in rpcClients)
                {
                    var type = client.GetReflector().GetMemberInfo().AsType();
                    i.AddSingleton(type, j =>
                    {
                        return j.GetRequiredService<IClientProxyCreater>().Create(type);
                    });
                }
                i.TryAddSingleton<IRpcClientInvokerFactory>(j => new RpcClientInvokerFactory(rpcClients, j.GetRequiredService<IRpcClientFactory>()));
            });
        }

        public static IEnumerable<(Type Service, Type Implementation)> GetAllHasAttributeTypes<Attribute>()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(i =>
                {
                    try
                    {
                        return i.GetExportedTypes();
                    }
                    catch
                    {
                        return new Type[0];
                    }
                })
                .SelectMany(i =>
                {
                    var reflector = i.GetReflector();
                    var services = i.GetInterfaces()
                    .Where(j => j.GetReflector().IsDefined<RpcAttribute>())
                    .Select(j => (Service: j, Implementation: i));
                    return i.IsInterface && reflector.IsDefined<RpcAttribute>()
                        ? services.Union(new (Type Service, Type Implementation)[1] { (Service: i, Implementation: null) })
                        : services;
                })
                .Distinct();
        }

        public static (IEnumerable<(Type Service, Type Implementation)> RpcServices, IEnumerable<Type> RpcClients)
            GetAllRpcServicesAndClients(IEnumerable<(Type Service, Type Implementation)> services)
        {
            var groups = services.GroupBy(i => i.Service)
                .ToArray();
            var clients = new List<Type>();
            var rpcServices = new List<(Type Service, Type Implementation)>();
            foreach (var group in groups)
            {
                foreach (var kv in group)
                {
                    if (kv.Implementation == null)
                    {
                        clients.Add(kv.Service);
                    }
                    else if (kv.Implementation.GetReflector().GetMemberInfo().IsClass)
                    {
                        rpcServices.Add(kv);
                    }
                }
            }
            return (RpcServices: rpcServices, RpcClients: clients.Where(i => !rpcServices.Any(j => i == j.Service)));
        }
    }
}