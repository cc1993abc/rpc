using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Metadata
{
    public class RpcMetadata : IRpcMetadata
    {
        private readonly HashSet<Type> clients;
        private readonly HashSet<Type> services;
        private Dictionary<string, (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType)> rpcMethods;
        private Dictionary<MethodInfo, (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, string servantName, string funcName, ParameterInfo[] allParameters)> rpcClientMethods;

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

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName)
        {
            return rpcMethods[$"{servantName}.{funcName}"];
        }

        public (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, string servantName, string funcName, ParameterInfo[] allParameters) FindRpcMethod(MethodInfo method)
        {
            return rpcClientMethods[method];
        }

        public void Init(IServiceProvider provider)
        {
            var handler = provider.GetRequiredService<IRpcMetadataHandler>();
            rpcMethods = new Dictionary<string, (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType)>(StringComparer.OrdinalIgnoreCase);
            var tempMethods = RpcServices.Select(i => i.service)
                .Union(Clients)
                .SelectMany(i =>
                {
                    var (servantName, codec, version) = handler.FindRpcInfo(i.GetReflector().FullDisplayName);
                    return i.GetMethods().Select(j => (name: $"{servantName}.{j.Name}", method: j, codec: codec, version: version, serviceType: i));
                })
                .Distinct();

            foreach (var item in tempMethods)
            {
                if (!rpcMethods.ContainsKey(item.name))
                {
                    rpcMethods.Add(item.name,
                        (item.method, item.method.GetReflector().IsDefined<OnewayAttribute>(),
                            item.method.GetParameters().Where(i => i.IsOut).ToArray(), item.codec, item.version, item.serviceType));
                }
            }

            rpcClientMethods = Clients.SelectMany(i =>
            {
                var (servantName, codec, version) = handler.FindRpcInfo(i.GetReflector().FullDisplayName);
                return i.GetMethods().Select(j =>
                {
                    var allParameters = j.GetParameters();
                    return (method: j, isOneway: j.GetReflector().IsDefined<OnewayAttribute>(), outParameters: allParameters.Where(x => x.IsOut).ToArray(),
                    codec: codec, version: version, servantName: servantName, funcName: j.Name, allParameters: allParameters);
                });
            }).ToDictionary(i => i.method);
        }
    }
}