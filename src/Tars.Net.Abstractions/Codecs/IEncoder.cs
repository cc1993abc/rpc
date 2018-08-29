using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IEncoder
    {
        object EncodeRequest(Request req);

        object EncodeResponse(Response message);
    }
}