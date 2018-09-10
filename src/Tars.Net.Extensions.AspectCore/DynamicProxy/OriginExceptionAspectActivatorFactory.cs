using AspectCore.DynamicProxy;
using System;

namespace Tars.Net.Extensions.AspectCore.DynamicProxy
{
    [NonAspect]
    public sealed class OriginExceptionAspectActivatorFactory : IAspectActivatorFactory
    {
        private readonly IAspectContextFactory _aspectContextFactory;
        private readonly IAspectBuilderFactory _aspectBuilderFactory;

        public OriginExceptionAspectActivatorFactory(IAspectContextFactory aspectContextFactory, IAspectBuilderFactory aspectBuilderFactory)
        {
            _aspectContextFactory = aspectContextFactory ?? throw new ArgumentNullException(nameof(aspectContextFactory));
            _aspectBuilderFactory = aspectBuilderFactory ?? throw new ArgumentNullException(nameof(aspectBuilderFactory));
        }

        public IAspectActivator Create()
        {
            return new OriginExceptionAspectActivator(_aspectContextFactory, _aspectBuilderFactory);
        }
    }
}