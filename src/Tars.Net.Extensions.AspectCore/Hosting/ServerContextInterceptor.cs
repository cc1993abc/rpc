using AspectCore.DynamicProxy;
using System.Threading.Tasks;

namespace Tars.Net.Hosting
{
    public class ServerContextInterceptor : AbstractInterceptor
    {
        public override int Order { get; set; } = 0;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var serverContext = ServerContext.Current.Context;
            foreach (var item in serverContext)
            {
                if (item.Key.StartsWith("#"))
                {
                    continue;
                }
                else if (context.AdditionalData.ContainsKey(item.Key))
                {
                    context.AdditionalData[item.Key] = item.Value.ToString();
                }
                else
                {
                    context.AdditionalData.Add(item.Key, item.Value.ToString());
                }
            }
            try
            {
                await next(context);
            }
            finally
            {
                foreach (var item in context.AdditionalData)
                {
                    if (item.Key.StartsWith("#"))
                    {
                        continue;
                    }
                    else if (serverContext.ContainsKey(item.Key))
                    {
                        serverContext[item.Key] = item.Value.ToString();
                    }
                    else
                    {
                        serverContext.Add(item.Key, item.Value.ToString());
                    }
                }
            }
        }
    }
}