using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IDecoder
    {
        Request DecodeRequest(dynamic input);

        void DecodeRequestContent(Request req);

        Response DecodeResponse(dynamic input);

        void DecodeResponseContent(Response resp);
    }
}