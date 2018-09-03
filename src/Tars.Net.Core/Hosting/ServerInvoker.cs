using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerInvoker : IServerInvoker
    {
        private readonly IDictionary<string, IDictionary<string, Action<Request, Response>>> invokers;
        private readonly IServiceProvider provider;
        private readonly IContentDecoder decoder;

        public ServerInvoker(IRpcMetadata rpcMetadata, IServiceProvider provider, IContentDecoder decoder)
        {
            invokers = CreateInvokersMap(rpcMetadata.RpcServices);
            this.provider = provider;
            this.decoder = decoder;
        }

        private IDictionary<string, IDictionary<string, Action<Request, Response>>> CreateInvokersMap(IEnumerable<(Type service, Type implementation)> rpcServices)
        {
            var dictionary = new Dictionary<string, IDictionary<string, Action<Request, Response>>>(StringComparer.OrdinalIgnoreCase);
            foreach (var (service, implementation) in rpcServices)
            {
                var attribute = service.GetReflector().GetCustomAttribute<RpcAttribute>();
                if (dictionary.ContainsKey(attribute.ServantName))
                {
                    continue;
                }
                dictionary.Add(attribute.ServantName, CreateFuncs(service, implementation));
            }
            return dictionary;
        }

        private IDictionary<string, Action<Request, Response>> CreateFuncs(Type service, Type implementation)
        {
            var dictionary = new Dictionary<string, Action<Request, Response>>(StringComparer.OrdinalIgnoreCase);
            foreach (var method in service.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (dictionary.ContainsKey(method.Name))
                {
                    continue;
                }
                var reflector = method.GetReflector();
                var codec = service.GetReflector().GetCustomAttribute<RpcAttribute>().Codec;
                var isOneway = reflector.IsDefined<OnewayAttribute>();
                var parameters = method.GetParameters();
                var outParameters = parameters.Where(i => i.IsOut).ToArray();
                dictionary.Add(method.Name, (req, resp) =>
                {
                    req.Codec = codec;
                    req.ParameterTypes = parameters;
                    decoder.DecodeRequestContent(req);
                    var serviceInstance = provider.GetService(service);
                    var context = new ServerContext();
                    ServerContext.Current = context;
                    try
                    {
                        resp.ReturnValue = reflector.Invoke(serviceInstance, req.Parameters);
                    }
                    finally
                    {
                        resp.Context = context.Context;
                    }
                    if (isOneway) return;
                    var returnParameters = new object[outParameters.Length];
                    var index = 0;
                    foreach (var item in outParameters)
                    {
                        if (index >= returnParameters.Length)
                        {
                            break;
                        }

                        returnParameters[index++] = req.Parameters[item.Position];
                    }
                    resp.ReturnValueType = method.ReturnParameter;
                    resp.ReturnParameterTypes = outParameters;
                    resp.ReturnParameters = returnParameters;
                    resp.Codec = codec;
                });
            }
            return dictionary;
        }

        public void Invoke(Request req, Response resp)
        {
            if (!invokers.TryGetValue(req.ServantName, out IDictionary<string, Action<Request, Response>> funcs))
            {
                throw new TarsException(RpcStatusCode.ServerNoServantErr, $"no found servant, serviceName[{ req.ServantName }]");
            }
            else if (!funcs.TryGetValue(req.FuncName, out Action<Request, Response> action))
            {
                throw new TarsException(RpcStatusCode.ServerNoFuncErr, $"no found methodInfo, serviceName[{ req.ServantName }], methodName[{req.FuncName}]");
            }
            else
            {
                action(req, resp);
            }
        }
    }
}