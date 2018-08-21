using System;
using System.Threading.Tasks;

namespace Tars.Csharp.Hosting
{
    public interface IServerHost
    {
        IServiceProvider Provider { get; }

        Task RunAsync(Func<Task> stopFunc);
    }
}