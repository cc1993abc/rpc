using AspectCore.DynamicProxy;
using Moq;
using System.Collections.Generic;
using Tars.Net.Clients;
using Xunit;

namespace Tars.Net.UT.AspectCore
{
    public class AspectClientsExtensionsTest
    {
        [Fact]
        public void IsRpcClientWhenContextHasContext_IsRpcClientShouldBeTrue()
        {
            var context = new Mock<AspectContext>();
            context.Setup(i => i.AdditionalData).Returns(new Dictionary<string, object>() { { AspectClientsExtensions.Context_IsRpcClient, true } });
            Assert.True(context.Object.IsRpcClient());
        }

        [Fact]
        public void IsRpcClientWhenContextNoContext_IsRpcClientShouldBeFalse()
        {
            var context = new Mock<AspectContext>();
            context.Setup(i => i.AdditionalData).Returns(new Dictionary<string, object>());
            Assert.False(context.Object.IsRpcClient());
        }

        [Fact]
        public void SetContextFromContextToAdditionalDataShouldCopyRight()
        {
            var additionalData = new Dictionary<string, object>();
            var context = new Dictionary<string, string>();
            additionalData.Add("old", "old");
            context.Add(AspectClientsExtensions.Context_IsRpcClient, "true");
            context.Add("old", "new");
            context.Add("id", "newid");
            additionalData.SetContext(context);
            Assert.False(additionalData.ContainsKey(AspectClientsExtensions.Context_IsRpcClient));
            Assert.Equal("new", additionalData["old"]);
            Assert.Equal("newid", additionalData["id"]);
        }

        [Fact]
        public void SetContextFromAdditionalDataToContextShouldCopyRight()
        {
            var additionalData = new Dictionary<string, object>();
            var context = new Dictionary<string, string>
            {
                { "old", "old" }
            };
            additionalData.Add(AspectClientsExtensions.Context_IsRpcClient, "true");
            additionalData.Add("old", "new");
            additionalData.Add("id", "newid");
            context.SetContext(additionalData);
            Assert.False(context.ContainsKey(AspectClientsExtensions.Context_IsRpcClient));
            Assert.Equal("new", context["old"]);
            Assert.Equal("newid", context["id"]);
        }
    }
}