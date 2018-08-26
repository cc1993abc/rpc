using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerInvoker
    {
        (object returnValue, object[] returnParameters, Codec codec) Invoke(Request msg);
    }
}