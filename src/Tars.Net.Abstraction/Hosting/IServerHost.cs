using System;
using System.Threading.Tasks;

namespace Tars.Net.Hosting
{
    public interface IServerHost
    {
        IServiceProvider Provider { get; }

        Task RunAsync(Func<Task> stopFunc);
    }
}