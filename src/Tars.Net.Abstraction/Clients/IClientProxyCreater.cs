using System;

namespace Tars.Net.Clients.Proxy
{
    public interface IClientProxyCreater
    {
        object Create(Type type);
    }
}