using System;

namespace Tars.Net.Clients
{
    public interface IClientProxyCreator
    {
        object Create(Type type);
    }
}