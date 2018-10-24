using Tars.Net.Codecs;

namespace Tars.Net.Configurations
{
    public class ServiceConfiguration
    {
        public string Servant { get; set; } = string.Empty;

        public string Interface { get; set; } = string.Empty;

        public Codec Codec { get; set; } = Codec.Tars;

        public short CodecVersion { get; set; } = 3;
    }
}