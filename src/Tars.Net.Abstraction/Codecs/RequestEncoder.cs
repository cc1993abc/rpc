using DotNetty.Buffers;
using Tars.Net.Metadata;

namespace Tars.Net.Codecs
{
    public abstract class RequestEncoder
    {
        public abstract IByteBuffer Encode(Request req, Codec codec);
    }
}