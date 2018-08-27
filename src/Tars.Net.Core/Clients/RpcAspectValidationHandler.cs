using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Tars.Net.Attributes;

namespace Tars.Net.Clients
{
    [NonAspect]
    public class RpcAspectValidationHandler : IAspectValidationHandler
    {
        public int Order => -1;

        public bool Invoke(AspectValidationContext context, AspectValidationDelegate next)
        {
            var method = context.Method;
            if (method.DeclaringType.GetReflector().IsDefined<RpcAttribute>())
            {
                return true;
            }
            return next(context);
        }
    }
}