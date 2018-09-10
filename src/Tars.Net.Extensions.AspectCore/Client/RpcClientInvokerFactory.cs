using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class RpcClientInvokerFactory
    {
        private readonly IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> invokers;
        private readonly IRpcClientFactory clientFactory;
        private readonly IContentDecoder decoder;

        public RpcClientInvokerFactory(IRpcMetadata rpcMetadata, IRpcClientFactory clientFactory, IContentDecoder decoder)
        {
            invokers = CreateRpcClientInvokers(rpcMetadata.Clients);
            this.clientFactory = clientFactory;
            this.decoder = decoder;
        }

        public IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> CreateRpcClientInvokers(IEnumerable<Type> rpcClients)
        {
            var dictionary = new Dictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>>();
            foreach (var item in rpcClients)
            {
                var attribute = item.GetCustomAttribute<RpcAttribute>();
                foreach (var method in item.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    var isOneway = method.GetReflector().IsDefined<OnewayAttribute>();
                    var outParameters = method.GetParameters().Where(i => i.IsOut).ToArray();
                    dictionary.Add(method, async (context, next) =>
                    {
                        var req = new Request()
                        {
                            ServantName = attribute.ServantName,
                            FuncName = method.Name,
                            Parameters = context.Parameters,
                            Codec = attribute.Codec,
                            IsOneway = isOneway
                        };
                        req.Context.SetContext(context.AdditionalData);
                        var resp = await clientFactory.SendAsync(req);
                        context.AdditionalData.SetContext(resp.Context);
                        context.ReturnValue = resp.ReturnValue;
                        if (isOneway)
                        {
                            await context.Complete();
                        }
                        else
                        {
                            resp.ReturnValueType = method.ReturnParameter;
                            resp.ReturnParameterTypes = outParameters;
                            decoder.DecodeResponseContent(resp);
                            object[] returnParameters = resp.ReturnParameters;
                            if (returnParameters != null && returnParameters.Length > 0)
                            {
                                var index = 0;
                                foreach (var outP in outParameters)
                                {
                                    if (index >= returnParameters.Length)
                                    {
                                        break;
                                    }

                                    req.Parameters[outP.Position] = returnParameters[index++];
                                }
                            }
                        }
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