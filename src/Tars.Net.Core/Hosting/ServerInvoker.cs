using AspectCore.Extensions.Reflection;
using System;
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

        public void Invoke(Request req, Response resp)
        {
            if (req.Mehtod == null)
            {
                throw new TarsException(RpcStatusCode.ServerNoFuncErr, $"No found methodInfo, serviceName[{ req.ServantName }], methodName[{req.FuncName}]");
            }

            var serviceInstance = provider.GetService(req.ServiceType);
            var context = new ServerContext();
            ServerContext.Current = context;
            try
            {
                resp.ReturnValue = req.Mehtod.GetReflector().Invoke(serviceInstance, req.Parameters);
            }
            finally
            {
                resp.Context = context.Context;
            }
            if (req.IsOneway)
            {
                return;
            }

            var index = 0;
            foreach (var item in req.ReturnParameterTypes)
            {
                resp.ReturnParameters[index++] = req.Parameters[item.Position];
            }
        }
    }
}