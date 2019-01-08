using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class TarsClientConfig
    {
        public Endpoint Endpoint { set; get; }
        public int SyncInvokeTimeout { set; get; }
        public int AsyncInvokeTimeout { set; get; }
        public int Connections { get; set; }
        public int ConnectTimeout { get; set; }
        public int CorePoolSize { get; set; }
        public int MaxPoolSize { get; set; }
        public int KeepAliveTime { get; set; }
        public int QueueSize { get; set; }
        public Encoding Charset { set; get; }
    }
}
