using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;

namespace Tars.Net.Metadata
{
    public class Response
    {
        #region Packet

        public short Version { get; set; }

        public int MessageType { get; set; }

        public int RequestId { get; set; }

        public string ServantName { get; set; }

        public string FuncName { get; set; }

        public object Buffer { get; set; }

        public int Timeout { get; set; }

        public IDictionary<string, string> Context { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Status { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Ret
        /// </summary>
        public RpcStatusCode ResultStatusCode { get; set; }

        public string ResultDesc { get; set; }

        #endregion Packet

        public object ReturnValue { get; set; }

        public object[] ReturnParameters { get; set; }

        public Codec Codec { get; set; }
        public ParameterInfo[] ReturnParameterTypes { get; set; }
        public ParameterInfo ReturnValueType { get; set; }
    }
}