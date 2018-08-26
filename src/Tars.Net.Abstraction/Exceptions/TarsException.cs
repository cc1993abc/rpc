using System;
using System.Collections.Generic;
using System.Text;
using Tars.Net.Metadata;

namespace Tars.Net.Exceptions
{
    public class TarsException : Exception
    {
        public RpcStatusCode RpcStatusCode { get; set; }

        public TarsException(RpcStatusCode returnStatusCode, Exception ex) : base(ex.Message, ex)
        {
            RpcStatusCode = returnStatusCode;
        }

        public TarsException(RpcStatusCode returnStatusCode, string message) : base(message)
        {
            RpcStatusCode = returnStatusCode;
        }
    }
}
