using Tars.Net.Codecs;

namespace Tars.Net.Configurations
{
    public class ServiceConfiguration
    {
        public string Servant { get; set; }

        public string Interface { get; set; }

        public Codec Codec { get; set; }

        public short CodecVersion { get; set; }
    }
}