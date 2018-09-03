using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Tars.Net.Clients;

namespace TcpCommon
{
    public class BusinessException: Exception
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }

    public class BusinessExceptionInterceptorAttribute : AbstractInterceptorAttribute
    {

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (context.IsRpcClient())
            {
                await next(context);
                if (context.AdditionalData.ContainsKey("Code"))
                {
                    throw new BusinessException()
                    {
                        Code = context.AdditionalData["Code"].ToString(),
                        Message = context.AdditionalData["Message"].ToString(),
                    };
                }
            }
            else
            {
                try
                {
                    await next(context);
                }
                catch (BusinessException ex)
                {
                    context.AdditionalData["Code"] = ex.Code;
                    context.AdditionalData["Message"] = ex.Message;
                }
            }
        }
    }
}
