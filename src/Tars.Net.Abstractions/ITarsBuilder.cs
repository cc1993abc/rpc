using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Tars.Net
{
    public interface ITarsBuilder
    {
        IServiceCollection Services { get; }

        IEnumerable<Type> Clients { get; }

        IEnumerable<(Type Service, Type Implementation)> RpcServices { get; }
    }
}