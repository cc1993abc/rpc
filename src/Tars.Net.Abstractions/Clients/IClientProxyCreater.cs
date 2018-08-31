using System;

namespace Tars.Net.Clients
{
    public interface IClientProxyCreater
    {
        object Create(Type type);
    }
}