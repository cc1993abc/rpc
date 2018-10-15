using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class ClientCallBack : IClientCallBack
    {
        private readonly ConcurrentDictionary<int, (TaskCompletionSource<Response> task, (string servantName, string funcName) rpcMethod)> callBacks 
            = new ConcurrentDictionary<int, (TaskCompletionSource<Response> task, (string servantName, string funcName) rpcMethod)>();
        private int callBackId = 0;

        public void CallBack(Response msg)
        {
            if (callBacks.TryRemove(msg.RequestId, out (TaskCompletionSource<Response> task, (string servantName, string funcName) rpcMethod) source))
            {
                source.task.SetResult(msg);
            }
        }

        public (string servantName, string funcName)? FindRpcMethod(int callBackId)
        {
            if (callBacks.TryGetValue(callBackId, out (TaskCompletionSource<Response> task, (string servantName, string funcName) rpcMethod) tSource))
            {
                return tSource.rpcMethod;
            }
            else
            {
                return null;
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
                callBacks.TryRemove(id, out (TaskCompletionSource<Response> task, (string servantName, string funcName) rpcMethod) tSource);
                if (!source.Task.IsCompleted)
                {
                    source.TrySetException(new TarsException(RpcStatusCode.AsyncCallTimeout, $"Call {servantName}.{funcName} timeout."));
                }
            });
            var info = (source, (servantName, funcName));
            callBacks.AddOrUpdate(id, info, (x, y) => info);
            tokenSource.CancelAfter(timeout * 1000);
            return source.Task;
        }
    }
}