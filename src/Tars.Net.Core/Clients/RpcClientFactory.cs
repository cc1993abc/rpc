using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly IContentDecoder decoder;
        private readonly RpcConfiguration configuration;
        private readonly IClientCallBack callBack;
        private readonly Dictionary<RpcProtocol, IRpcClient> clients;

        public RpcClientFactory(IEnumerable<IRpcClient> rpcClients, IContentDecoder decoder, RpcConfiguration configuration, IClientCallBack callBack)
        {
            this.decoder = decoder;
            this.configuration = configuration;
            this.callBack = callBack;
            clients = rpcClients.ToDictionary(i => i.Protocol);
        }

        public async Task<object> SendAsync(string servantName, string funcName, ParameterInfo[] outParameters, ParameterInfo returnValueType, bool isOneway, Codec codec, object[] parameters)
        {
            var req = new Request()
            {
                RequestId = callBack.NewCallBackId(),
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
                var response = await callBack.NewCallBackTask(req.RequestId, req.Timeout, servantName, funcName);
                CheckResponse(response);
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

        private void CheckResponse(Response response)
        {
            if (response.ResultStatusCode != RpcStatusCode.ServerSuccess)
            {
                throw new TarsException(response.ResultStatusCode, response.ResultDesc);
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
                await client.SendAsync(config.EndPoint, req);
            }
            else
            {
                throw new NotSupportedException($"No find Rpc client which supported {Enum.GetName(typeof(RpcProtocol), config.Protocol)}");
            }
        }
    }
}