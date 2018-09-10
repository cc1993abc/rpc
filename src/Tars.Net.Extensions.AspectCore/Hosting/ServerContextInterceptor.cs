using AspectCore.DynamicProxy;
using System.Threading.Tasks;
using Tars.Net.Clients;

namespace Tars.Net.Hosting
{
    public class ServerContextInterceptor : AbstractInterceptor
    {
        public override int Order { get; set; } = 0;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var serverContext = ServerContext.Current?.Context;
            serverContext?.SetContext(context.AdditionalData);
            try
            {
                await next(context);
            }
            finally
            {
                serverContext?.SetContext(context.AdditionalData);
            }
        }
    }
}