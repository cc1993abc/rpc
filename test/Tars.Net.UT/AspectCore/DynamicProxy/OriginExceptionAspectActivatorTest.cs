using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Extensions.AspectCore.DynamicProxy;
using Tars.Net.UT.Core.Hosting;
using Xunit;

namespace Tars.Net.UT.AspectCore.DynamicProxy
{
    public class OriginExceptionAspectActivatorTest
    {
        private readonly IAspectActivator sut;
        private readonly object proxy;

        public OriginExceptionAspectActivatorTest()
        {
            var services = new ServiceCollection()
                .ConfigureDynamicProxy()
                .BuildDynamicProxyServiceProvider();
            proxy = services.GetRequiredService<IProxyGenerator>().CreateInterfaceProxy(typeof(ITestServer), new TestServer());
            var builder = new Mock<IAspectBuilderFactory>();
            builder.Setup(i => i.Create(It.IsAny<AspectContext>()))
            .Returns<AspectContext>(i =>
            {
                var mock = new Mock<IAspectBuilder>();
                mock.Setup(j => j.Build()).Returns((c) =>
                 {
                     if (c.Parameters[0].ToString() == "1")
                     {
                         return Task.FromException(new Exception("TEST"));
                     }
                     else if (c.Parameters[0].ToString() == "2")
                     {
                         var source = new TaskCompletionSource<object>();
                         var tokenSource = new CancellationTokenSource();
                         tokenSource.Token.Register(() =>
                         {
                             source.TrySetException(new Exception("timeout"));
                         });
                         tokenSource.CancelAfter(1000);
                         return source.Task;
                     }
                     else if (c.Parameters[0].ToString() == "4")
                     {
                         c.ReturnValue = Task.FromResult(4);
                         return Task.CompletedTask;
                     }
                     else if (c.Parameters[0].ToString() == "5")
                     {
                         c.ReturnValue = Task.CompletedTask;
                         return Task.CompletedTask;
                     }
                     else if (c.Parameters[0].ToString() == "6")
                     {
                         c.ReturnValue = 3;
                         return Task.CompletedTask;
                     }
                     else if (c.Parameters[0].ToString() == "7")
                     {
                         c.ReturnValue = new ValueTask<int>(7);
                         return Task.CompletedTask;
                     }
                     else
                     {
                         return Task.CompletedTask;
                     }
                 });
                return mock.Object;
            });
            sut = new OriginExceptionAspectActivatorFactory(services.GetRequiredService<IAspectContextFactory>(), builder.Object).Create();
        }

        private AspectActivatorContext CreateContext(int p)
        {
            return new AspectActivatorContext(typeof(ITestServer).GetMethod("GetVNoP", new Type[] { typeof(int) }),
                typeof(TestServer).GetMethod("GetVNoP", new Type[] { typeof(int) }),
                proxy.GetType().GetMethod("GetVNoP", new Type[] { typeof(int) }),
                new TestServer(), proxy, new object[] { p });
        }

        [Fact]
        public void InvokeWhenFaultedShouldThrowEx()
        {
            AspectActivatorContext context = CreateContext(1);
            var ex = Assert.Throws<Exception>(() => sut.Invoke<object>(context));
            Assert.Equal("TEST", ex.Message);
        }

        [Fact]
        public void InvokeWhenNotCompletedShouldThrowEx()
        {
            AspectActivatorContext context = CreateContext(2);
            var ex = Assert.Throws<Exception>(() => sut.Invoke<object>(context));
            Assert.Equal("timeout", ex.Message);
        }

        [Fact]
        public void InvokeWhenCompletedShouldNoEx()
        {
            AspectActivatorContext context = CreateContext(3);
            sut.Invoke<object>(context);
        }

        [Fact]
        public async void InvokeTaskWhenTaskWithResultShouldBe4()
        {
            AspectActivatorContext context = CreateContext(4);
            Assert.Equal(4, await sut.InvokeTask<int>(context));
        }

        [Fact]
        public async void InvokeTaskWhenTaskShouldBe0()
        {
            AspectActivatorContext context = CreateContext(5);
            Assert.Equal(0, await sut.InvokeTask<int>(context));
        }

        [Fact]
        public async void InvokeTaskWhenNotTaskShouldThrowEx()
        {
            AspectActivatorContext context = CreateContext(6);
            var ex = await Assert.ThrowsAsync<AspectInvocationException>(() => sut.InvokeTask<int>(context));
            Assert.Equal("Unable to cast object of type 'System.Int32' to type 'System.Threading.Tasks.Task`1[System.Int32]'.", ex.InnerException.Message);
        }

        [Fact]
        public async void InvokeValueTaskWhenValueTaskShouldBe7()
        {
            AspectActivatorContext context = CreateContext(7);
            Assert.Equal(7, await sut.InvokeValueTask<int>(context));
        }
    }
}