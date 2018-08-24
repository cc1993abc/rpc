using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Attributes;
using System.Linq;

namespace Tars.Net.Clients
{
    public class RpcClientInvokerFactory : IRpcClientInvokerFactory
    {
        private readonly IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> invokers;

        public RpcClientInvokerFactory(IEnumerable<Type> rpcClients)
        {
            invokers = CreateRpcClientInvokers(rpcClients);
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
                        var rpc = context.ServiceProvider.GetRequiredService<IRpcClient>();
                        context.ReturnValue = await rpc.SendAsync(attribute.ServantName, method.Name, outParameters, isOneway, attribute.Codec, context.Parameters);
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