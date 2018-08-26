using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerInvoker
    {
        (object returnValue, object[] returnParameters) Invoke(Request msg);
    }
}