using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Codecs;

namespace Tars.Net.Clients
{
    public interface IRpcClientFactory
    {
        Task<object> SendAsync(string servantName, string name, ParameterInfo[] outParameters, ParameterInfo returnValueType, bool isOneway, Codec codec, object[] parameters);
    }
}