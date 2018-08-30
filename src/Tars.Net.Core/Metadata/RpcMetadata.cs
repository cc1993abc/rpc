using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Tars.Net.Attributes;

namespace Tars.Net.Metadata
{
    public class RpcMetadata : IRpcMetadata
    {
        public IEnumerable<Type> Clients { get; }

        public IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }

        public RpcMetadata()
        {
            var all = GetAllHasAttributeTypes();
            var (rpcServices, rpcClients) = GetAllRpcServicesAndClients(all);
            Clients = rpcClients;
            RpcServices = rpcServices;
        }

        public (IEnumerable<(Type Service, Type Implementation)> RpcServices, IEnumerable<Type> RpcClients)
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
                    var services = i.GetInterfaces()
                    .Where(j => j.GetReflector().IsDefined<RpcAttribute>())
                    .Select(j => (Service: j, Implementation: i));
                    return i.IsInterface && reflector.IsDefined<RpcAttribute>()
                        ? services.Union(new (Type Service, Type Implementation)[1] { (Service: i, Implementation: null) })
                        : services;
                })
                .Distinct();
        }
    }
}