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
        private readonly IDictionary<string, IDictionary<string, Func<Request, (object, object[], Codec)>>> invokers;
        private readonly IServiceProvider provider;
        private readonly IContentDecoder decoder;

        public ServerInvoker(IEnumerable<(Type service, Type implementation)> rpcServices, IServiceProvider provider, IContentDecoder decoder)
        {
            invokers = CreateInvokersMap(rpcServices);
            this.provider = provider;
            this.decoder = decoder;
        }

        private IDictionary<string, IDictionary<string, Func<Request, (object, object[], Codec)>>> CreateInvokersMap(IEnumerable<(Type service, Type implementation)> rpcServices)
        {
            var dictionary = new Dictionary<string, IDictionary<string, Func<Request, (object, object[], Codec)>>>(StringComparer.OrdinalIgnoreCase);
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

        private IDictionary<string, Func<Request, (object, object[], Codec)>> CreateFuncs(Type service, Type implementation)
        {
            var dictionary = new Dictionary<string, Func<Request, (object, object[], Codec)>>(StringComparer.OrdinalIgnoreCase);
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
                dictionary.Add(method.Name, (msg) =>
                {
                    msg.Codec = codec;
                    msg.ParameterTypes = parameters;
                    decoder.DecodeRequestContent(msg);
                    var serviceInstance = provider.GetService(service);
                    var returnValue = reflector.Invoke(serviceInstance, msg.Parameters);
                    var returnParameters = new object[outParameters.Length];
                    var index = 0;
                    foreach (var item in outParameters)
                    {
                        if (index >= returnParameters.Length)
                        {
                            break;
                        }

                        returnParameters[index++] = msg.Parameters[item.Position];
                    }
                    return (returnValue, returnParameters, codec);
                });
            }
            return dictionary;
        }

        public (object returnValue, object[] returnParameters, Codec codec) Invoke(Request msg)
        {
            if (!invokers.TryGetValue(msg.ServantName, out IDictionary<string, Func<Request, (object, object[], Codec)>> funcs))
            {
                throw new TarsException(RpcStatusCode.ServerNoServantErr, $"no found servant, serviceName[{ msg.ServantName }]");
            }
            else if (!funcs.TryGetValue(msg.FuncName, out Func<Request, (object, object[], Codec)> func))
            {
                throw new TarsException(RpcStatusCode.ServerNoFuncErr, $"no found methodInfo, serviceName[{ msg.ServantName }], methodName[{msg.FuncName}]");
            }
            else
            {
                return func(msg);
            }
        }
    }
}