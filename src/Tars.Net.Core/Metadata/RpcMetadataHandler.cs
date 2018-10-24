using System;
using System.Collections.Generic;
using System.Linq;
using Tars.Net.Codecs;
using Tars.Net.Configurations;

namespace Tars.Net.Metadata
{
    public class RpcMetadataHandler : IRpcMetadataHandler
    {
        private readonly Dictionary<string, (string Servant, Codec Codec, short CodecVersion)> rpcInfos;

        public RpcMetadataHandler(RpcConfiguration configuration)
        {
            rpcInfos = configuration.ClientConfig.Values.Union(configuration.ServiceConfig)
                .Select(i => Tuple.Create(i.Interface, (i.Servant, i.Codec, i.CodecVersion)))
                .ToDictionary(i => i.Item1, i => i.Item2, StringComparer.OrdinalIgnoreCase);
        }

        public (string servantName, Codec codec, short version) FindRpcInfo(string serviceTypeName)
        {
            return rpcInfos.ContainsKey(serviceTypeName) 
                ? rpcInfos[serviceTypeName] 
                : (string.Empty, Codec.Tars, (short)3);
        }
    }
}