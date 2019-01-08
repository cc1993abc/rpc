using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Tars.Net.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDefaultTarsConfiguration(this IServiceCollection services)
        {
            services.TryAddSingleton(new ConfigurationManager());
            return services;
        }
    }
}
