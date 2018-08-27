using System;
using Tars.Net.Codecs;

namespace Tars.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RpcAttribute : Attribute
    {
        public RpcAttribute(string servantName, Codec codec = Codec.Tars, int timeout = 60)
        {
            ServantName = servantName;
            Codec = codec;
            Timeout = timeout;
        }

        public string ServantName { get; }
        public Codec Codec { get; }
        public int Timeout { get; }
    }
}