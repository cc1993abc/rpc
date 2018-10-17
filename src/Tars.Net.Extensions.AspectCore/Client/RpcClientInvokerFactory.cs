using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Attributes;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class RpcClientInvokerFactory
    {
        private readonly IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> invokers;
        private readonly IRpcClientFactory clientFactory;

        public RpcClientInvokerFactory(IRpcMetadata rpcMetadata, IRpcClientFactory clientFactory)
        {
            invokers = CreateRpcClientInvokers(rpcMetadata);
            this.clientFactory = clientFactory;
        }

        public IDictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>> CreateRpcClientInvokers(IRpcMetadata rpcMetadata)
        {
            var dictionary = new Dictionary<MethodInfo, Func<AspectContext, AspectDelegate, Task>>();
            foreach (var item in rpcMetadata.Clients)
            {
                var attribute = item.GetCustomAttribute<RpcAttribute>();
                foreach (var method in item.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    var (methodInfo, isOneway, outParameters, codec, version, serviceType) = rpcMetadata.FindRpcMethod(attribute.ServantName, method.Name);
                    var parameterTypes = methodInfo.GetParameters();
                    dictionary.Add(method, async (context, next) =>
                    {
                        var req = new Request()
                        {
                            ServantName = attribute.ServantName,
                            FuncName = method.Name,
                            Codec = codec,
                            IsOneway = isOneway,
                            ParameterTypes = parameterTypes,
                            ReturnParameterTypes = outParameters,
                            Parameters = context.Parameters,
                            Version = version
                        };
                        req.Context.SetContext(context.AdditionalData);
                        var resp = await clientFactory.SendAsync(req);
                        context.AdditionalData.SetContext(resp.Context);
                        if (isOneway)
                        {
                            await context.Complete();
                        }
                        else
                        {
                            context.ReturnValue = resp.ReturnValue;
                            resp.ReturnValueType = method.ReturnParameter;
                            resp.ReturnParameterTypes = outParameters;
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