using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Tars.Net.Attributes;

namespace Tars.Net.Metadata
{
    public class RpcMetadata : IRpcMetadata
    {
        private readonly HashSet<Type> clients;
        private readonly HashSet<Type> services;

        public IEnumerable<Type> Clients => clients;

        public IEnumerable<(Type service, Type implementation)> RpcServices { get; }

        public RpcMetadata()
        {
            var all = GetAllHasAttributeTypes();
            var (rpcServices, rpcClients) = GetAllRpcServicesAndClients(all);
            clients = new HashSet<Type>(rpcClients.Distinct());
            services = new HashSet<Type>(rpcServices.Select(i => i.service).Distinct());
            RpcServices = rpcServices;
        }

        public (IEnumerable<(Type service, Type implementation)> rpcServices, IEnumerable<Type> rpcClients)
            GetAllRpcServicesAndClients(IEnumerable<(Type service, Type implementation)> services)
        {
            var groups = services.GroupBy(i => i.service)
                .ToArray();
            var clientTypes = new List<Type>();
            var rpcServices = new List<(Type Service, Type Implementation)>();
            foreach (var group in groups)
            {
                foreach (var kv in group)
                {
                    if (kv.implementation == null)
                    {
                        clientTypes.Add(kv.service);
                    }
                    else if (kv.implementation.GetReflector().GetMemberInfo().IsClass)
                    {
                        rpcServices.Add(kv);
                    }
                }
            }
            return (rpcServices: rpcServices, rpcClients: clientTypes.Where(i => rpcServices.All(j => i != j.Service)));
        }

        public IEnumerable<(Type Service, Type Implementation)> GetAllHasAttributeTypes()
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
                    var serviceTypes = i.GetInterfaces()
                    .Where(j => j.GetReflector().IsDefined<RpcAttribute>())
                    .Select(j => (Service: j, Implementation: i));
                    return i.IsInterface && reflector.IsDefined<RpcAttribute>()
                        ? serviceTypes.Union(new (Type Service, Type Implementation)[1] { (Service: i, Implementation: null) })
                        : serviceTypes;
                })
                .Distinct();
        }

        public bool IsRpcServiceType(Type type)
        {
            return services.Contains(type);
        }

        public bool IsRpcClientType(Type type)
        {
            return clients.Contains(type);
        }
    }
}