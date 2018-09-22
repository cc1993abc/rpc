using System;
using System.Collections.Generic;
using System.Reflection;
using Tars.Net.Codecs;

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

        public object Buffer { get; set; }

        public int Timeout { get; set; }

        public IDictionary<string, string> Context { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Status { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion Packet

        public object[] Parameters { get; set; }

        public Codec Codec { get; set; }

        public bool IsOneway { get; set; }

        public ParameterInfo[] ParameterTypes { get; set; }

        public MethodInfo Mehtod { get; set; }

        public ParameterInfo[] ReturnParameterTypes { get; set; }

        public Type ServiceType { get; set; }

        public Response CreateResponse()
        {
            return new Response()
            {
                Version = Version,
                PacketType = PacketType,
                MessageType = MessageType,
                RequestId = RequestId,
                ServantName = ServantName,
                FuncName = FuncName,
                Timeout = Timeout,
                Buffer = null,
                ResultDesc = null,
                ResultStatusCode = RpcStatusCode.ServerSuccess,
                ReturnValue = null,
                ReturnParameters = ReturnParameterTypes == null ? null : new object[ReturnParameterTypes.Length],
                Codec = Codec.Tars,
                ReturnParameterTypes = ReturnParameterTypes,
                ReturnValueType = Mehtod?.ReturnParameter,
            };
        }
    }
}