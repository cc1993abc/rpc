using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class RpcClientFactory : IRpcClientFactory
    {
        private readonly Task<object> completedTask = Task.FromResult<object>(null);
        private readonly RequestEncoder encoder;
        private readonly ResponseDecoder decoder;
        private readonly RpcConfiguration configuration;
        private readonly Dictionary<RpcProtocol, IRpcClient> clients;

        private ConcurrentDictionary<int, TaskCompletionSource<Response>> callBacks = new ConcurrentDictionary<int, TaskCompletionSource<Response>>();
        private int requestId = 0;

        public RpcClientFactory(IEnumerable<IRpcClient> rpcClients, RequestEncoder encoder, ResponseDecoder decoder, RpcConfiguration configuration)
        {
            this.encoder = encoder;
            this.decoder = decoder;
            this.configuration = configuration;
            clients = rpcClients.ToDictionary(i => i.Protocol);
        }

        public async Task<object> SendAsync(string servantName, string funcName, ParameterInfo[] outParameters, bool isOneway, Codec codec, int timeout, object[] parameters)
        {
            var req = new Request()
            {
                RequestId = NewRequestId(),
                ServantName = servantName,
                FuncName = funcName,
                Timeout = timeout,
                Parameters = parameters,
                Codec = codec
            };
            await SendAsync(req);
            if (isOneway)
            {
                return completedTask;
            }
            else
            {
                var id = req.RequestId;
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
                    t.Result.Codec = codec;
                    decoder.DecodeResponseContent(t.Result);
                    object returnValue = t.Result.ReturnValue;
                    object[] returnParameters = t.Result.ReturnParameters;

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

        private async Task SendAsync(Request req)
        {
            if (!configuration.ClientConfig.TryGetValue(req.ServantName, out ClientConfiguration config))
            {
                throw new KeyNotFoundException($"No find Rpc client config for {req.ServantName}");
            }
            if (clients.TryGetValue(config.Protocol, out IRpcClient client))
            {
                await client.SendAsync(config.EndPoint, encoder.Encode(req));
            }
            else
            {
                throw new NotSupportedException($"No find Rpc client which supported {Enum.GetName(typeof(RpcProtocol), config.Protocol)}");
            }
        }

        public int NewRequestId()
        {
            return Interlocked.Increment(ref requestId);
        }

        public void SetResult(Response response)
        {
            if (callBacks.TryRemove(response.RequestId, out TaskCompletionSource<Response> source))
            {
                source.SetResult(response);
            }
        }
    }
}