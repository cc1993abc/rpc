using AspectCore.Extensions.Reflection;
using System;
using System.Threading.Tasks;
using Tars.Net.Diagnostics;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Hosting
{
    public class ServerInvoker : IServerInvoker
    {
        private readonly IServiceProvider provider;

        public ServerInvoker(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public async Task<Response> InvokeAsync(Request req, Response resp)
        {
            if (req.Mehtod == null)
            {
                resp.ResultStatusCode = RpcStatusCode.ServerNoFuncErr;
                resp.ResultDesc = $"No found methodInfo, serviceName[{ req.ServantName }], methodName[{req.FuncName}]";
                return resp;
            }

            var context = new ServerContext();
            ServerContext.Current = context;
            try
            {
                var serviceInstance = provider.GetService(req.ServiceType);
                resp.ReturnValue = req.Mehtod.GetReflector().Invoke(serviceInstance, req.Parameters);
                if (resp.ReturnValue is Task task)
                {
                    await task;
                }

                if (req.IsOneway)
                {
                    return resp;
                }

                var index = 0;
                foreach (var item in req.ReturnParameterTypes)
                {
                    resp.ReturnParameters[index++] = req.Parameters[item.Position];
                }
            }
            catch (TarsException ex)
            {
                resp.ResultStatusCode = ex.RpcStatusCode;
                resp.ResultDesc = ex.Message;
                ServerHandler.Diagnostic.HostingException(req, resp, ex);
            }
            catch (Exception ex)
            {
                resp.ResultStatusCode = RpcStatusCode.ServerUnknownErr;
                resp.ResultDesc = ex.Message;
                ServerHandler.Diagnostic.HostingException(req, resp, ex);
            }
            finally
            {
                resp.Context = context.Context;
            }

            return resp;
        }
    }
}