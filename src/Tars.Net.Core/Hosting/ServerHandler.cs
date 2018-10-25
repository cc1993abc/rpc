using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tars.Net.Diagnostics;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerHandler : IServerHandler
    {
        private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener(DiagnosticListenerExtensions.DiagnosticListenerName);
        private readonly IServerInvoker serverInvoker;

        public ServerHandler(IServerInvoker serverInvoker, IRpcMetadata rpcMetadata, IServiceProvider provider)
        {
            rpcMetadata.Init(provider);
            this.serverInvoker = serverInvoker;
        }

        public async Task<Response> ProcessAsync(Request req)
        {
            s_diagnosticListener.HostingRequest(req);
            var response = req.CreateResponse();
            try
            {
                if (!"tars_ping".Equals(req.FuncName, StringComparison.OrdinalIgnoreCase))
                {
                    response = await serverInvoker.InvokeAsync(req, response);
                }
            }
            catch (TarsException ex)
            {
                response.ResultStatusCode = ex.RpcStatusCode;
                response.ResultDesc = ex.Message;
                s_diagnosticListener.HostingException(req, response, ex);
            }
            catch (Exception ex)
            {
                response.ResultStatusCode = RpcStatusCode.ServerUnknownErr;
                response.ResultDesc = ex.Message;
                s_diagnosticListener.HostingException(req, response, ex);
            }
            finally
            {
                s_diagnosticListener.HostingResponse(req, response);
            }

            return response;
        }
    }
}