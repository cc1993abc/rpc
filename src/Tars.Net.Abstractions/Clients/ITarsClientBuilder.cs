using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net
{
    public interface ITarsClientBuilder
    {
        IServiceCollection Services { get; }

        IEnumerable<Type> Clients { get; }
    }
}