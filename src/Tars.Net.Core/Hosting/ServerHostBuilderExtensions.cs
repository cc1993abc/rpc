using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Clients.Proxy;

namespace Tars.Net.Hosting
{
    public static class ServerHostBuilderExtensions
    {
        private static void ReigsterRpcDep(IServiceCollection services)
        {
            services.TryAddSingleton<IClientProxyCreater, ClientProxyCreater>();
            services.TryAddSingleton<IRpcClientInvokeBuilder, RpcClientInvokeBuilder>();
        }

        public static IServerHostBuilder ReigsterRpc(this IServerHostBuilder builder, params Assembly[] assemblies)
        {
            var all = GetAllHasAttributeTypes<RpcAttribute>();
            var (RpcServices, RpcClients) = GetAllRpcServicesAndClients(all);
            return builder.ConfigureServices(i =>
            {
                ReigsterRpcDep(i);
                foreach (var (Service, Implementation) in RpcServices)
                {
                    i.TryAddSingleton(Service.GetMemberInfo().AsType(), Implementation.GetMemberInfo().AsType());
                }

                foreach (var client in RpcClients)
                {
                    var type = client.GetMemberInfo().AsType();
                    i.AddSingleton(type, j =>
                    {
                        return j.GetRequiredService<IClientProxyCreater>().Create(type);
                    });
                }
            });
        }

        public static IEnumerable<(TypeReflector Service, TypeReflector Implementation)> GetAllHasAttributeTypes<Attribute>()
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
                    .Select(j => j.GetReflector())
                    .Where(j => j.IsDefined<RpcAttribute>())
                    .Select(j => (Service: j, Implementation: reflector));
                    return i.IsInterface && reflector.IsDefined<RpcAttribute>()
                        ? services.Union(new (TypeReflector Service, TypeReflector Implementation)[1] { (Service: reflector, Implementation: null) })
                        : services;
                })
                .Distinct();
        }

        public static (IEnumerable<(TypeReflector Service, TypeReflector Implementation)> RpcServices, IEnumerable<TypeReflector> RpcClients)
            GetAllRpcServicesAndClients(IEnumerable<(TypeReflector Service, TypeReflector Implementation)> services)
        {
            var groups = services.GroupBy(i => i.Service)
                .ToArray();
            var clients = new List<TypeReflector>();
            var rpcServices = new List<(TypeReflector Service, TypeReflector Implementation)>();
            foreach (var group in groups)
            {
                foreach (var kv in group)
                {
                    if (kv.Implementation == null)
                    {
                        clients.Add(kv.Service);
                    }
                    else if (kv.Implementation.GetMemberInfo().IsClass)
                    {
                        rpcServices.Add(kv);
                    }
                }
            }
            return (RpcServices: rpcServices, RpcClients: clients.Where(i => !rpcServices.Any(j => i == j.Service)));
        }
    }
}