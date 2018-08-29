using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IDecoder<T>
    {
        Request DecodeRequest(T input);

        void DecodeRequestContent(Request req);

        Response DecodeResponse(T input);

        void DecodeResponseContent(Response resp);
    }
}