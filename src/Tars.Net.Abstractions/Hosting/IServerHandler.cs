using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public interface IServerHandler
    {
        Response Process(Request req);
    }
}