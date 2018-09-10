using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using Tars.Net.Extensions.AspectCore.DynamicProxy;
using Tars.Net.Hosting;
using Xunit;

namespace Tars.Net.UT.AspectCore
{
    public class HostBuilderExtensionsTest
    {
        [Fact]
        public void UseAopShouldConfigRight()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));
            builder.Setup(i => i.UseServiceProviderFactory(It.IsAny<IServiceProviderFactory<IServiceCollection>>()))
                .Callback<IServiceProviderFactory<IServiceCollection>>(i => i.CreateServiceProvider(i.CreateBuilder(services)));
            builder.Object.UseAop();
            Assert.IsType<OriginExceptionAspectActivatorFactory>(services.BuildServiceProvider().GetRequiredService<IAspectActivatorFactory>());
        }
    }
}