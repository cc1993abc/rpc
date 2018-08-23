using AspectCore.DynamicProxy;
using System;
using System.Reflection;

namespace Tars.Net.Clients.Proxy
{
    [NonAspect]
    public class ClientProxyAspectBuilderFactory : IAspectBuilderFactory
    {
        private readonly IInterceptorCollector interceptorCollector;
        private readonly IRpcClientFactory clientFactory;
        private readonly IAspectCaching aspectCaching;

        public ClientProxyAspectBuilderFactory(IInterceptorCollector interceptorCollector,
            IAspectCachingProvider aspectCachingProvider, IRpcClientFactory clientFactory)
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
            var invoker = clientFactory.GetClientInvoker(tuple.Item2);
            aspectBuilder.AddAspectDelegate(async (context, next) => await invoker.InvokeAsync(context));
            foreach (var interceptor in interceptorCollector.Collect(tuple.Item1, tuple.Item2))
            {
                aspectBuilder.AddAspectDelegate(interceptor.Invoke);
            }

            return aspectBuilder;
        }
    }
}