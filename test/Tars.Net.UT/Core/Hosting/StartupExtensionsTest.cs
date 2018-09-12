using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using Tars.Net.Hosting;
using Xunit;

namespace Tars.Net.UT.Core.Hosting
{
    public class Startup : IStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IStartup, Startup>();
        }
    }

    public class StartupExtensionsTest
    {
        [Fact]
        public void UseStartupShouldConfigureServicesRight()
        {
            var services = new ServiceCollection();
            var builder = new Mock<IHostBuilder>();
            builder.Setup(i => i.ConfigureServices(It.IsAny<Action<HostBuilderContext, IServiceCollection>>()))
                .Callback<Action<HostBuilderContext, IServiceCollection>>(action => action(null, services));
            builder.Object.UseStartup<Startup>();
            Assert.IsType<Startup>(services.BuildServiceProvider().GetRequiredService<IStartup>());
        }
    }
}