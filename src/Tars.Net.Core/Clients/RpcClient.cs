using System;
using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Codecs;

namespace Tars.Net.Clients
{
    public class RpcClient : IRpcClient
    {
        public Task<object> SendAsync(string servantName, string name, ParameterInfo[] outParameters, bool isOneway, Codec codec, object[] parameters)
        {
            //Todo : add RpcSender
            throw new NotImplementedException();
        }
    }
}