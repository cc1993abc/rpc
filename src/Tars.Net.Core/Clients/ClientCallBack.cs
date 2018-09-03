using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class ClientCallBack : IClientCallBack
    {
        private readonly ConcurrentDictionary<int, TaskCompletionSource<Response>> callBacks = new ConcurrentDictionary<int, TaskCompletionSource<Response>>();
        private int callBackId = 0;

        public void CallBack(Response msg)
        {
            if (callBacks.TryRemove(msg.RequestId, out TaskCompletionSource<Response> source))
            {
                source.SetResult(msg);
            }
        }

        public int NewCallBackId()
        {
            return Interlocked.Increment(ref callBackId);
        }

        public Task<Response> NewCallBackTask(int id, int timeout, string servantName, string funcName)
        {
            var source = new TaskCompletionSource<Response>();
            var tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(() =>
            {
                callBacks.TryRemove(id, out TaskCompletionSource<Response> tSource);
                if (!source.Task.IsCompleted)
                {
                    source.TrySetException(new TarsException(RpcStatusCode.AsyncCallTimeout, $"Call {servantName}.{funcName} timeout."));
                }
            });
            callBacks.AddOrUpdate(id, source, (x, y) => source);
            tokenSource.CancelAfter(timeout * 1000);
            return source.Task;
        }
    }
}