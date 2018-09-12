using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Tars.Net.Metadata
{
    public static class RpcMetadataExtensions
    {
        public static IRpcMetadata GetRpcMetadata(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(i => i.ServiceType == typeof(IRpcMetadata));
            if (descriptor == null)
            {
                descriptor = new ServiceDescriptor(typeof(IRpcMetadata), new RpcMetadata());
                services.Add(descriptor);
            }
            return (IRpcMetadata)descriptor.ImplementationInstance;
        }
    }
}