using System;
using System.Collections.Generic;

namespace Tars.Net.Metadata
{
    public interface IRpcMetadata
    {
        IEnumerable<Type> Clients { get; }

        IEnumerable<(Type service, Type implementation)> RpcServices { get; }

        bool IsRpcServiceType(Type type);

        bool IsRpcClientType(Type type);
    }
}