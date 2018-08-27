using DotNetty.Buffers;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public interface IEncoder
    {
        IByteBuffer EncodeRequest(Request req);

        IByteBuffer EncodeResponse(Response message);
    }
}