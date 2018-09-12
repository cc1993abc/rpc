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

        public Response Process(Request req)
        {
            var response = req.CreateResponse();
            try
            {
                if (!"tars_ping".Equals(req.FuncName, StringComparison.OrdinalIgnoreCase))
                {
                    serverInvoker.Invoke(req, response);
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