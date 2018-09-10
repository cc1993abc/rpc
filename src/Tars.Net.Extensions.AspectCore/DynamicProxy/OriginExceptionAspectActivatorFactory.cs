using AspectCore.DynamicProxy;
using System;

namespace Tars.Net.Extensions.AspectCore.DynamicProxy
{
    [NonAspect]
    public sealed class OriginExceptionAspectActivatorFactory : IAspectActivatorFactory
    {
        private readonly IAspectContextFactory aspectContextFactory;
        private readonly IAspectBuilderFactory aspectBuilderFactory;
    
        public OriginExceptionAspectActivatorFactory(IAspectContextFactory aspectContextFactory, IAspectBuilderFactory aspectBuilderFactory)
        {
            this.aspectContextFactory = aspectContextFactory;
            this.aspectBuilderFactory = aspectBuilderFactory;
        }

        public IAspectActivator Create()
        {
            return new OriginExceptionAspectActivator(aspectContextFactory, aspectBuilderFactory);
        }
    }
}