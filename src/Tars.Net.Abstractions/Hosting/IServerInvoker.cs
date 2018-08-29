using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerInvoker
    {
        void Invoke(Request req, Response resp);
    }
}