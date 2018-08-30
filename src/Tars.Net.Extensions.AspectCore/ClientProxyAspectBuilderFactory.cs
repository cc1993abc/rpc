using AspectCore.DynamicProxy;
using System;
using System.Reflection;

namespace Tars.Net.Extensions.AspectCore
{
    [NonAspect]
    public class ClientProxyAspectBuilderFactory : IAspectBuilderFactory
    {
        private readonly IInterceptorCollector interceptorCollector;
        private readonly IRpcClientInvokerFactory clientFactory;
        private readonly IAspectCaching aspectCaching;

        public ClientProxyAspectBuilderFactory(IInterceptorCollector interceptorCollector,
            IAspectCachingProvider aspectCachingProvider, IRpcClientInvokerFactory clientFactory)
        {
            if (aspectCachingProvider == null)
            {
                throw new ArgumentNullException(nameof(aspectCachingProvider));
            }
            this.interceptorCollector =
                interceptorCollector ?? throw new ArgumentNullException(nameof(interceptorCollector));
            this.clientFactory = clientFactory;
            aspectCaching = aspectCachingProvider.GetAspectCaching(nameof(ClientProxyAspectBuilderFactory));
        }

        public IAspectBuilder Create(AspectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return (IAspectBuilder)aspectCaching.GetOrAdd(GetKey(context.ServiceMethod, context.ImplementationMethod), key => Create((Tuple<MethodInfo, MethodInfo>)key));
        }

        private object GetKey(MethodInfo serviceMethod, MethodInfo implementationMethod)
        {
            return Tuple.Create(serviceMethod, implementationMethod);
        }

        private IAspectBuilder Create(Tuple<MethodInfo, MethodInfo> tuple)
        {
            var aspectBuilder = new AspectBuilder(context => context.Complete(), null);
            var func = clientFactory.GetClientInvoker(tuple.Item1);
            aspectBuilder.AddAspectDelegate(func);
            foreach (var interceptor in interceptorCollector.Collect(tuple.Item1, tuple.Item2))
            {
                aspectBuilder.AddAspectDelegate(interceptor.Invoke);
            }

            return aspectBuilder;
        }
    }
}