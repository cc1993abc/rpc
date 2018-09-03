using System;
using System.Collections.Generic;

namespace Tars.Net.Metadata
{
    public interface IRpcMetadata
    {
        IEnumerable<Type> Clients { get; }

        IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }

        bool IsRpcServiceType(Type type);

        bool IsRpcClientType(Type type);
    }
}