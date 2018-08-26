using DotNetty.Buffers;
using System;
using System.Collections.Generic;

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

        public IByteBuffer Buffer { get; set; }

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
    }
}