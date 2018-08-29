using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Tars.Net.Attributes;
using Tars.Net.Clients;
using Tars.Net.Clients.Proxy;

namespace Tars.Net
{
    public static class RpcExtensions
    {
        public static IServiceCollection ReigsterRpcDependency(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientProxyCreater, ClientProxyCreater>();
            services.TryAddSingleton<ClientProxyAspectBuilderFactory, ClientProxyAspectBuilderFactory>();
            //todo: add Decoder and Encoder
            //services.TryAddSingleton<RequestDecoder, TestRequestDecoder>();
            //services.TryAddSingleton<RequestEncoder, TestRequestEncoder>();
            //services.TryAddSingleton<ResponseDecoder, TestResponseDecoder>();
            //services.TryAddSingleton<ResponseEncoder, TestResponseEncoder>();
            return services;
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