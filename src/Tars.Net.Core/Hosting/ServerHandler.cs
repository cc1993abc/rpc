using System;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerHandler : IServerHandler
    {
        private readonly IServerInvoker serverInvoker;

        public ServerHandler(IServerInvoker serverInvoker)
        {
            this.serverInvoker = serverInvoker;
        }

        public Response Process(Request msg)
        {
            var response = msg.CreateResponse();
            try
            {
                if ("tars_ping".Equals(msg.FuncName, StringComparison.OrdinalIgnoreCase))
                {
                    //TODO: Define tars' own default value
                    response.Buffer = "none";
                }
                else
                {
                    var (returnValue, returnParameters, codec) = serverInvoker.Invoke(msg);
                    response.ReturnValue = returnValue;
                    response.ReturnParameters = returnParameters;
                    response.Codec = codec;
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