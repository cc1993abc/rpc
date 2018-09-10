using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tars.Net.Configurations;
using Tars.Net.Metadata;

namespace Tars.Net.Clients
{
    public class RpcClientFactory : IRpcClientFactory
    {
        private readonly RpcConfiguration configuration;
        private readonly IClientCallBack callBack;
        private readonly Dictionary<RpcProtocol, IRpcClient> clients;

        public RpcClientFactory(IEnumerable<IRpcClient> rpcClients, RpcConfiguration configuration, IClientCallBack callBack)
        {
            this.configuration = configuration;
            this.callBack = callBack;
            clients = rpcClients.ToDictionary(i => i.Protocol);
        }

        public async Task<Response> SendAsync(Request req)
        {
            req.RequestId = callBack.NewCallBackId();
            await SendRequestAsync(req);
            if (req.IsOneway)
            {
                return new Response();
            }
            else
            {
                var response = await callBack.NewCallBackTask(req.RequestId, req.Timeout, req.ServantName, req.FuncName);
                response.CheckResultStatus();
                response.Codec = req.Codec;
                return response;
            }
        }

        public async Task SendRequestAsync(Request req)
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