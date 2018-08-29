using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IContentDecoder
    {
        void DecodeRequestContent(Request req);

        void DecodeResponseContent(Response resp);
    }
}