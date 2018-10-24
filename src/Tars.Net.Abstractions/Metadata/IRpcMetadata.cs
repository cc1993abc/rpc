using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;

namespace Tars.Net.Metadata
{
    public interface IRpcMetadata
    {
        IEnumerable<Type> Clients { get; }

        IEnumerable<(Type service, Type implementation)> RpcServices { get; }

        bool IsRpcServiceType(Type type);

        bool IsRpcClientType(Type type);

        (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, Type serviceType) FindRpcMethod(string servantName, string funcName);

        (MethodInfo method, bool isOneway, ParameterInfo[] outParameters, Codec codec, short version, string servantName, string funcName, ParameterInfo[] allParameters) FindRpcMethod(MethodInfo method);

        void Init(IServiceProvider provider);
    }
}