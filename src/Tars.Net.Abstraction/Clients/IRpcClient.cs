using System.Reflection;
using System.Threading.Tasks;
using Tars.Net.Codecs;

namespace Tars.Net.Clients
{
    public interface IRpcClient
    {
        Task<object> SendAsync(string servantName, string name, ParameterInfo[] outParameters, bool isOneway, Codec codec, object[] parameters);
    }
}