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
        private Bootstrap bootstrap = new Bootstrap();
        private readonly ConcurrentDictionary<EndPoint, IChannel> channels = new ConcurrentDictionary<EndPoint, IChannel>();
        private readonly ResponseDecoder decoder;

        public RpcProtocol Protocol => RpcProtocol.Tcp;

        public LibuvTcpClient(RpcConfiguration configuration, ResponseDecoder decoder)
        {
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var lengthFieldLength = configuration.LengthFieldLength;
                    channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                         configuration.MaxFrameLength, 0, lengthFieldLength, -1 * lengthFieldLength, 0, true));
                    channel.Pipeline.AddLast(decoder);
                }));
            this.decoder = decoder;
        }

        public async Task SendAsync(EndPoint endPoint, IByteBuffer request)
        {
            var channel = await ConnectAsync(endPoint);
            await channel.WriteAndFlushAsync(request);
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