using System;
using Tars.Net.Codecs;

namespace Tars.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RpcAttribute : Attribute
    {
        public RpcAttribute(string servantName, Codec codec = Codec.Tars)
        {
            ServantName = servantName;
            Codec = codec;
        }

        public string ServantName { get; }

        public Codec Codec { get; }

        public short Version { get; set; } = 0x03;
    }
}