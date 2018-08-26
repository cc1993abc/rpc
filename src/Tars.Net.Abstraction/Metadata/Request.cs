using DotNetty.Buffers;
using System;
using System.Collections.Generic;

namespace Tars.Net.Metadata
{
    public class Request
    {
        #region Packet

        public short Version { get; set; }

        public byte PacketType { get; set; }

        public int MessageType { get; set; }

        public int RequestId { get; set; }

        public string ServantName { get; set; }

        public string FuncName { get; set; }

        public IByteBuffer Buffer { get; set; }

        public int Timeout { get; set; }

        public IDictionary<string, string> Context { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Status { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion Packet

        public object[] Parameters { get; set; }

        public Response CreateResponse()
        {
            return new Response()
            {
                Version = Version,
                MessageType = MessageType,
                RequestId = RequestId,
                ServantName = ServantName,
                FuncName = FuncName,
                Timeout = Timeout
            };
        }
    }
}