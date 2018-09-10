using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net.Hosting
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}