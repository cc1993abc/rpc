using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty.Udp;
using Tars.Net.Hosting.Udp;
using Tars.Net.Metadata;

namespace Tars.Net.Clients.Udp
{
    public class UdpClient : IRpcClient
    {
        private readonly MultithreadEventLoopGroup group = new MultithreadEventLoopGroup();
        private readonly Bootstrap bootstrap = new Bootstrap();
        private IChannel channel;
        private readonly IEncoder<IByteBuffer> encoder;
        public RpcProtocol Protocol => RpcProtocol.Udp;

        public UdpClient(RpcConfiguration configuration, IDecoder<IByteBuffer> decoder, IEncoder<IByteBuffer> encoder, IClientCallBack callBack)
        {
            this.encoder = encoder;
            bootstrap
                .Group(group)
                .Channel<SocketDatagramChannel>()
                .LocalAddress(configuration.UdpCilentPort)
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var lengthFieldLength = configuration.LengthFieldLength;
                    channel.Pipeline.AddLast(new UdpClientHandler());
                    channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                         configuration.MaxFrameLength, 0, lengthFieldLength, 0, lengthFieldLength, true));
                    channel.Pipeline.AddLast(new ResponseDecoder(decoder), new DatagramPacketEncoder<IByteBuffer>(new UdpLengthFieldPrepender(lengthFieldLength)),
                        new DotNettyClientHandler(callBack));
                }));
        }

        public async Task SendAsync(EndPoint endPoint, Request request)
        {
            channel = await ConnectAsync();
            var req = encoder.EncodeRequest(request);
            await channel.WriteAndFlushAsync(new DatagramPacket(req, channel.LocalAddress, endPoint));
        }

        private async Task<IChannel> ConnectAsync()
        {
            if (channel == null || !channel.Active)
            {
                channel = await bootstrap.BindAsync();
            }
            return channel;
        }

        public Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan shutdownTimeout)
        {
            return group.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
        }
    }
}