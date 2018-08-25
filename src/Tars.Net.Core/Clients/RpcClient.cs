using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Codecs;

namespace Tars.Net.Clients
{
    public class RpcClient : IRpcClient
    {
        private ConcurrentDictionary<int, TaskCompletionSource<Response>> callBacks = new ConcurrentDictionary<int, TaskCompletionSource<Response>>();
        private int requestId = 0;

        public async Task<object> SendAsync(string servantName, string funcName, ParameterInfo[] outParameters, bool isOneway, Codec codec, int timeout, object[] parameters)
        {
            var req = new Request()
            {
                Id = NewRequestId(),
                ServantName = servantName,
                FuncName = funcName,
                IsOneway = isOneway
            };
            await SendAsync(req);
            if (isOneway)
            {
                return Task.FromResult<object>(null);
            }
            else
            {
                var id = req.Id;
                var source = new TaskCompletionSource<Response>();
                var tokenSource = new CancellationTokenSource();
                tokenSource.Token.Register(() =>
                {
                    source.TrySetCanceled(tokenSource.Token);
                    callBacks.TryRemove(id, out TaskCompletionSource<Response> tSource);
                });
                callBacks.AddOrUpdate(id, source, (x, y) => source);
                tokenSource.CancelAfter(timeout);
                return source.Task.ContinueWith(t =>
                {
                    var (returnValue, returnParameters) = DecodeResponse(t.Result);
                    if (returnParameters == null)
                    {
                        return returnValue;
                    }

                    var index = 0;
                    foreach (var item in outParameters)
                    {
                        if (index >= returnParameters.Length)
                        {
                            break;
                        }

                        parameters[item.Position] = returnParameters[index++];
                    }
                    return returnValue;
                });
            }
        }

        private (object, object[]) DecodeResponse(Response result)
        {
            throw new NotImplementedException();
        }

        private Task SendAsync(Request req)
        {
            throw new NotImplementedException();
        }

        public int NewRequestId()
        {
            return Interlocked.Increment(ref requestId);
        }

        public void SetResult(Response response)
        {
            if (callBacks.TryRemove(response.Id, out TaskCompletionSource<Response> source))
            {
                source.SetResult(response);
            }
        }
    }
}