using Tars.Net.Codecs;

namespace Tars.Net.Metadata
{
    public interface IRpcMetadataHandler
    {
        (string servantName, Codec codec, short version) FindRpcInfo(string serviceTypeName);
    }
}