using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IEncoder<T>
    {
        T EncodeRequest(Request req);

        T EncodeResponse(Response message);
    }
}