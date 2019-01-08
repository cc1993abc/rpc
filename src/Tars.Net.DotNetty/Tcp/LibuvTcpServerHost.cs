using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty.Hosting;

namespace Tars.Net.Hosting.Tcp
{
    public class LibuvTcpServerHost : IHostedService
    {
        public IServiceProvider Provider { get; }
        private DispatcherEventLoopGroup bossGroup;
        private WorkerEventLoopGroup workerGroup;
        private IChannel bootstrapChannel;
        #region before
        private readonly RpcConfiguration configuration;
        private readonly ILogger<LibuvTcpServerHost> logger;
        private readonly IDecoder<IByteBuffer> decoder;
        private readonly IEncoder<IByteBuffer> encoder;
        private readonly DotNettyServerHandler handler;
    
        public LibuvTcpServerHost(IServiceProvider provider, RpcConfiguration configuration,
    ILogger<LibuvTcpServerHost> logger, IDecoder<IByteBuffer> decoder, IEncoder<IByteBuffer> encoder,
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
        public LibuvTcpServerHost(IServiceProvider provider, ServantAdapterConfig servantAdapterConfig)
        {
            Provider = provider;
            this.servantAdapterConfig = servantAdapterConfig;
            this.logger = provider.GetRequiredService<ILogger<LibuvTcpServerHost>>();
            this.decoder = provider.GetRequiredService<IDecoder<IByteBuffer>>();
            this.encoder = provider.GetRequiredService<IEncoder<IByteBuffer>>();
            this.handler = provider.GetRequiredService<DotNettyServerHandler>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            bossGroup = new DispatcherEventLoopGroup();
            workerGroup = new WorkerEventLoopGroup(bossGroup, servantAdapterConfig.Threads);
            ServerBootstrap bootstrap = new ServerBootstrap();
            bootstrap.Group(bossGroup, workerGroup);
            bootstrap.Channel<TcpServerChannel>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }

            bootstrap
               .Option(ChannelOption.SoBacklog, servantAdapterConfig.MaxConnections)
               .Option(ChannelOption.TcpNodelay, servantAdapterConfig.TcpNoDelay)
               .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
               {
                   IChannelPipeline pipeline = channel.Pipeline;
                   pipeline.AddLast(new TcpHandler(Provider.GetRequiredService<ILogger<TcpHandler>>()));
                   var lengthFieldLength = servantAdapterConfig.LengthFieldLength;
                   pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                        servantAdapterConfig.MaxFrameLength, 0, lengthFieldLength, 0, lengthFieldLength, true));
                   pipeline.AddLast(new RequestDecoder(decoder), new LengthFieldPrepender(lengthFieldLength), new ResponseEncoder(encoder), handler);
               }));
            logger.LogInformation($"Server start at {IPAddress.Any}:{servantAdapterConfig.Endpoint.Port}.");
            return bootstrap.BindAsync(servantAdapterConfig.Endpoint.Host, servantAdapterConfig.Endpoint.Port)
                .ContinueWith(i => bootstrapChannel = i.Result);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await bootstrapChannel.CloseAsync();
            var quietPeriod = servantAdapterConfig.QuietPeriodTimeSpan;
            var shutdownTimeout = servantAdapterConfig.ShutdownTimeoutTimeSpan;
            await workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            await bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            foreach (var item in Provider.GetServices<IRpcClient>())
            {
                await item.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            }
        }
    }
}