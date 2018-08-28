using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Exceptions;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class RpcClientFactory : IRpcClientFactory
    {
        private readonly Task<object> completedTask = Task.FromResult<object>(null);
        private readonly IEncoder encoder;
        private readonly IDecoder decoder;
        private readonly RpcConfiguration configuration;
        private readonly Dictionary<RpcProtocol, IRpcClient> clients;

        private ConcurrentDictionary<int, TaskCompletionSource<Response>> callBacks = new ConcurrentDictionary<int, TaskCompletionSource<Response>>();
        private int requestId = 0;

        public RpcClientFactory(IEnumerable<IRpcClient> rpcClients, IEncoder encoder, IDecoder decoder, RpcConfiguration configuration)
        {
            this.encoder = encoder;
            this.decoder = decoder;
            this.configuration = configuration;
            clients = rpcClients.ToDictionary(i => i.Protocol);
            foreach (var client in clients.Values)
            {
                client.SetClientCallBack(this);
            }
        }

        public async Task<object> SendAsync(string servantName, string funcName, ParameterInfo[] outParameters, ParameterInfo returnValueType, bool isOneway, Codec codec, object[] parameters)
        {
            var req = new Request()
            {
                RequestId = NewRequestId(),
                ServantName = servantName,
                FuncName = funcName,
                Parameters = parameters,
                Codec = codec,
                IsOneway = isOneway
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
                    callBacks.TryRemove(id, out TaskCompletionSource<Response> tSource);
                    if (!source.Task.IsCompleted)
                    {
                        source.TrySetException(new TarsException(RpcStatusCode.AsyncCallTimeout, $"Call {servantName}.{funcName} timeout."));
                    }
                });
                callBacks.AddOrUpdate(id, source, (x, y) => source);
                tokenSource.CancelAfter(req.Timeout * 1000);
                var response = await source.Task;
                response.Codec = codec;
                response.ReturnValueType = returnValueType;
                response.ReturnParameterTypes = outParameters;
                decoder.DecodeResponseContent(response);
                object[] returnParameters = response.ReturnParameters;
                if (returnParameters == null)
                {
                    return response.ReturnValue;
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
                return response.ReturnValue;
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
                req.Timeout = config.Timeout;
                await client.SendAsync(config.EndPoint, encoder.EncodeRequest(req));
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

        public void CallBack(Response msg)
        {
            if (callBacks.TryRemove(msg.RequestId, out TaskCompletionSource<Response> source))
            {
                source.SetResult(msg);
            }
        }
    }
}