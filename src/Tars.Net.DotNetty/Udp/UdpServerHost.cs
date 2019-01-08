using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty.Hosting;
using Tars.Net.DotNetty.Udp;

namespace Tars.Net.Hosting.Udp
{
    public class UdpServerHost : IHostedService
    {
        public IServiceProvider Provider { get; }
        private MultithreadEventLoopGroup workerGroup;
        private IChannel bootstrapChannel;
        #region before
        private readonly RpcConfiguration configuration;
        private readonly ILogger<UdpServerHost> logger;
        private readonly IDecoder<IByteBuffer> decoder;
        private readonly IEncoder<IByteBuffer> encoder;
        private readonly DotNettyServerHandler handler;
        public UdpServerHost(IServiceProvider provider, RpcConfiguration configuration,
            ILogger<UdpServerHost> logger, IDecoder<IByteBuffer> decoder, IEncoder<IByteBuffer> encoder,
            DotNettyServerHandler handler)
        {
            Provider = provider;
            this.configuration = configuration;
            this.logger = logger;
            this.decoder = decoder;
            this.encoder = encoder;
            this.handler = handler;
        }
        #endregion
        private ServantAdapterConfig servantAdapterConfig;
        public UdpServerHost(IServiceProvider provider, ServantAdapterConfig servantAdapterConfig)
        {
            Provider = provider;
            this.servantAdapterConfig = servantAdapterConfig;
            this.logger = provider.GetRequiredService<ILogger<UdpServerHost>>();
            this.decoder = provider.GetRequiredService<IDecoder<IByteBuffer>>();
            this.encoder = provider.GetRequiredService<IEncoder<IByteBuffer>>();
            this.handler = provider.GetRequiredService<DotNettyServerHandler>();
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            workerGroup = new MultithreadEventLoopGroup(servantAdapterConfig.Threads);
            var bootstrap = new Bootstrap();
            bootstrap.Group(workerGroup)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
               .Handler(new ActionChannelInitializer<IChannel>(channel =>
               {
                   IChannelPipeline pipeline = channel.Pipeline;
                   pipeline.AddLast(new UdpHandler(Provider.GetRequiredService<ILogger<UdpHandler>>()));
                   var lengthFieldLength = servantAdapterConfig.LengthFieldLength;
                   pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                        servantAdapterConfig.MaxFrameLength, 0, lengthFieldLength, 0, lengthFieldLength, true));
                   pipeline.AddLast(new RequestDecoder(decoder), new UdpLengthFieldPrepender(lengthFieldLength), new ResponseEncoder(encoder), handler);
               }));
            logger.LogInformation($"Server start at {IPAddress.Any}:{servantAdapterConfig.Endpoint.Port}.");
            bootstrapChannel = await bootstrap.BindAsync(servantAdapterConfig.Endpoint.Host, servantAdapterConfig.Endpoint.Port);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (bootstrapChannel != null)
            {
                await bootstrapChannel.CloseAsync();
            }

            var quietPeriod = servantAdapterConfig.QuietPeriodTimeSpan;
            var shutdownTimeout = servantAdapterConfig.ShutdownTimeoutTimeSpan;
            await workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            foreach (var item in Provider.GetServices<IRpcClient>())
            {
                await item.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            }
        }
    }
}