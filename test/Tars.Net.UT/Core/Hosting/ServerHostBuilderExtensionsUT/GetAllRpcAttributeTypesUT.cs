using AspectCore.Extensions.Reflection;
using System.Linq;
using Tars.Net.Attributes;
using Tars.Net.Hosting;
using Tars.Net.UT.Core.Hosting.ServerHostBuilderExtensionsUT;
using Xunit;

namespace Tars.Net.UT.Core.Hosting.ServerHostBuilderExtensionsUT1
{
    public partial class TestPartialClass_AttributeTypeScan : ITestInherited_AttributeTypeScan
    {
    }

    public interface ITestInherited_AttributeTypeScan : ITestAttributeTypeScan
    { }
}

namespace Tars.Net.UT.Core.Hosting.ServerHostBuilderExtensionsUT
{
    [Rpc]
    public interface ITestAttributeTypeScan
    {
    }

    [Rpc]
    public interface ITestRpcInterface : ITestAttributeTypeScan
    {
    }

    public partial class TestPartialClass_AttributeTypeScan : ITestAttributeTypeScan
    {
    }

    public class GetAllRpcAttributeTypesUT
    {
        private (TypeReflector Service, TypeReflector Implementation)[] result;

        public GetAllRpcAttributeTypesUT()
        {
            result = ServerHostBuilderExtensions.GetAllHasAttributeTypes<RpcAttribute>().ToArray();
        }

        [Fact]
        public void ShouldBe5ITestAttributeTypeScan()
        {
            Assert.Equal(5, result.Where(i => i.Service.Name == "ITestAttributeTypeScan").Count());
        }

        [Fact]
        public void ShouldBeTwoPartialClass()
        {
            Assert.Equal(2, result.Where(i => i.Implementation?.Name == "TestPartialClass_AttributeTypeScan").Count());
            Assert.Empty(result.Where(i => i.Service.Name == "TestPartialClass_AttributeTypeScan"));
        }

        [Fact]
        public void ShouldBeOneInheritedInterface()
        {
            Assert.Single(result.Where(i => i.Implementation?.Name == "ITestInherited_AttributeTypeScan"));
            Assert.Empty(result.Where(i => i.Service.Name == "ITestInherited_AttributeTypeScan"));
        }

        [Fact]
        public void ShouldBeOneITestRpcInterface()
        {
            Assert.Single(result.Where(i => i.Implementation?.Name == "ITestRpcInterface"));
            Assert.Single(result.Where(i => i.Service.Name == "ITestRpcInterface"));
        }
    }

    public class GetAllRpcServicesAndClientsUT
    {
        private TypeReflector[] clients;
        private (TypeReflector Service, TypeReflector Implementation)[] services;

        public GetAllRpcServicesAndClientsUT()
        {
            var all = ServerHostBuilderExtensions.GetAllHasAttributeTypes<RpcAttribute>().ToArray();
            var (RpcServices, RpcClients) = ServerHostBuilderExtensions.GetAllRpcServicesAndClients(all);
            clients = RpcClients.ToArray();
            services = RpcServices.ToArray();
        }

        [Fact]
        public void ClientsShouldBe1()
        {
            Assert.Single(clients);
            Assert.Equal("ITestRpcInterface", clients.First().Name);
        }

        [Fact]
        public void ClientsShouldBe2()
        {
            Assert.Equal(2, services.Length);
            Assert.Equal(2, services.Where(i => i.Implementation.Name == "TestPartialClass_AttributeTypeScan").Count());
        }
    }
}