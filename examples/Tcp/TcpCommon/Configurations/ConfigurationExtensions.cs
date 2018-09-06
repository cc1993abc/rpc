using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tars.Net.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, string key = "Host")
        {

            services.TryAddSingleton(j =>
            {
                var config = new RpcConfiguration();
                j.GetRequiredService<IConfiguration>().Bind(key, config);
                return config;
            });

            return services;
        }
    }
}