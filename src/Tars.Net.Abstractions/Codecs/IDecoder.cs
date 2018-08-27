using DotNetty.Buffers;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IDecoder
    {
        Request DecodeRequest(IByteBuffer input);

        void DecodeRequestContent(Request req);

        Response DecodeResponse(IByteBuffer input);

        void DecodeResponseContent(Response resp);
    }
}