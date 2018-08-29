using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.Metadata;

namespace Tars.Net.Clients.Tcp
{
    public class LibuvTcpClient : IRpcClient
    {
        private MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
        private readonly Bootstrap bootstrap = new Bootstrap();
        private readonly ConcurrentDictionary<EndPoint, IChannel> channels = new ConcurrentDictionary<EndPoint, IChannel>();
        private readonly IEncoder<IByteBuffer> encoder;
        public RpcProtocol Protocol => RpcProtocol.Tcp;

        public LibuvTcpClient(RpcConfiguration configuration, IDecoder<IByteBuffer> decoder, IEncoder<IByteBuffer> encoder, IClientCallBack callBack)
        {
            this.encoder = encoder;
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var lengthFieldLength = configuration.LengthFieldLength;
                    channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                         configuration.MaxFrameLength, 0, lengthFieldLength, 0, lengthFieldLength, true));
                    channel.Pipeline.AddLast(new ResponseDecoder(decoder), new LengthFieldPrepender(lengthFieldLength),
                        new DotNettyClientHandler(callBack));
                }));
        }

        public async Task SendAsync(EndPoint endPoint, Request request)
        {
            var channel = await ConnectAsync(endPoint);
            var req = encoder.EncodeRequest(request);
            await channel.WriteAndFlushAsync(req);
        }

        private async Task<IChannel> ConnectAsync(EndPoint endPoint)
        {
            var channel = await bootstrap.ConnectAsync(endPoint);
            channels.AddOrUpdate(endPoint, channel, (x, y) => channel);
            return channel;
        }

        public Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout)
        {
            return group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}