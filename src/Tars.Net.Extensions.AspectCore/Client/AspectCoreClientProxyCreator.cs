﻿using AspectCore.DynamicProxy;
using System;
using Tars.Net.Extensions.AspectCore.DynamicProxy;

namespace Tars.Net.Clients
{
    public class AspectCoreClientProxyCreator : IClientProxyCreator
    {
        private readonly IProxyTypeGenerator generator;
        private readonly IAspectContextFactory contextFactory;
        private readonly ClientProxyAspectBuilderFactory aspectBuilderFactory;

        public AspectCoreClientProxyCreator(IProxyTypeGenerator generator, IAspectContextFactory contextFactory, ClientProxyAspectBuilderFactory aspectBuilderFactory)
        {
            this.generator = generator;
            this.contextFactory = contextFactory;
            this.aspectBuilderFactory = aspectBuilderFactory;
        }

        public object Create(Type type)
        {
            var proxyType = generator.CreateInterfaceProxyType(type);
            return Activator.CreateInstance(proxyType, new OriginExceptionAspectActivatorFactory(contextFactory, aspectBuilderFactory));
        }
    }
}