using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tars.Net.Diagnostics;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerHandler : IServerHandler
    {
        internal static readonly DiagnosticListener Diagnostic = new DiagnosticListener(DiagnosticListenerExtensions.DiagnosticListenerName);
        private readonly IServerInvoker serverInvoker;

        public ServerHandler(IServerInvoker serverInvoker, IRpcMetadata rpcMetadata, IServiceProvider provider)
        {
            rpcMetadata.Init(provider);
            this.serverInvoker = serverInvoker;
        }

        public async Task<Response> ProcessAsync(Request req)
        {
            Diagnostic.HostingRequest(req);
            var response = req.CreateResponse();
            try
            {
                if (!"tars_ping".Equals(req.FuncName, StringComparison.OrdinalIgnoreCase))
                {
                    response = await serverInvoker.InvokeAsync(req, response);
                }
            }
            finally
            {
                Diagnostic.HostingResponse(req, response);
            }

            return response;
        }
    }
}