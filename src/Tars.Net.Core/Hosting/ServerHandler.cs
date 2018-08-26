using DotNetty.Buffers;
using System;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerHandler : ServerHandlerBase
    {
        private readonly IServerInvoker serverInvoker;

        public ServerHandler(IServerInvoker serverInvoker)
        {
            this.serverInvoker = serverInvoker;
        }

        public override Response Process(Request msg)
        {
            var response = msg.CreateResponse();
            try
            {
                if ("tars_ping".Equals(msg.FuncName, StringComparison.OrdinalIgnoreCase))
                {
                    response.Buffer = Unpooled.Empty;
                }
                else
                {
                    var (returnValue, returnParameters) = serverInvoker.Invoke(msg);
                    response.ReturnValue = returnValue;
                    response.ReturnParameters = returnParameters;
                }
            }
            catch (TarsException ex)
            {
                response.ResultStatusCode = ex.RpcStatusCode;
                response.ResultDesc = ex.Message;
            }
            catch (Exception ex)
            {
                response.ResultStatusCode = RpcStatusCode.ServerUnknownErr;
                response.ResultDesc = ex.Message;
            }

            return response;
        }
    }
}