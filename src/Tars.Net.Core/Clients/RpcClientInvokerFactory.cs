using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Attributes;

namespace Tars.Net.Clients
{
    public class RpcClientInvokerFactory : IRpcClientInvokerFactory
    {
        private readonly IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> invokers;
        private readonly IRpcClientFactory clientFactory;

        public RpcClientInvokerFactory(IEnumerable<Type> rpcClients, IRpcClientFactory clientFactory)
        {
            invokers = CreateRpcClientInvokers(rpcClients);
            this.clientFactory = clientFactory;
        }

        public IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> CreateRpcClientInvokers(IEnumerable<Type> rpcClients)
        {
            var dictionary = new Dictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>>();
            foreach (var item in rpcClients)
            {
                var attribute = item.GetCustomAttribute<RpcAttribute>();
                var codec = item.GetCustomAttribute<RpcAttribute>();
                foreach (var method in item.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    var isOneway = method.GetReflector().IsDefined<OnewayAttribute>();
                    var outParameters = method.GetParameters().Where(i => i.IsOut).ToArray();
                    dictionary.Add(method, async (context, next) =>
                    {
                        context.ReturnValue = await clientFactory.SendAsync(attribute.ServantName, method.Name, outParameters, isOneway,
                            attribute.Codec, attribute.Timeout, context.Parameters);
                    });
                }
            }
            return dictionary;
        }

        public Func<AspectContext, AspectDelegate, Task> GetClientInvoker(MethodInfo method)
        {
            return invokers[method];
        }
    }
}