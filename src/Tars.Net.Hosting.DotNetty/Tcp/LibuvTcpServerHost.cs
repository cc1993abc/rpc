using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Hosting.Configurations;

namespace Tars.Net.Hosting.Tcp
{
    public class LibuvTcpServerHost : IServerHost
    {
        public IServiceProvider Provider { get; }

        private readonly HostConfiguration configuration;
        private readonly ILogger<LibuvTcpServerHost> logger;
        private readonly RequestDecoder decoder;
        private readonly ResponseEncoder encoder;
        private readonly ServerHandlerBase handler;
        private DispatcherEventLoopGroup bossGroup;
        private WorkerEventLoopGroup workerGroup;

        public LibuvTcpServerHost(IServiceProvider provider, HostConfiguration configuration,
            ILogger<LibuvTcpServerHost> logger, RequestDecoder decoder, ResponseEncoder encoder,
            ServerHandlerBase handler)
        {
            Provider = provider;
            this.configuration = configuration;
            this.logger = logger;
            this.decoder = decoder;
            this.encoder = encoder;
            this.handler = handler;
        }

        public async Task RunAsync(Func<Task> stopFunc)
        {
            bossGroup = new DispatcherEventLoopGroup();
            workerGroup = new WorkerEventLoopGroup(bossGroup);

            try
            {
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
                   .Option(ChannelOption.SoBacklog, configuration.SoBacklog)
                   .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                   {
                       IChannelPipeline pipeline = channel.Pipeline;
                       pipeline.AddLast(new TcpHandler());
                       var lengthFieldLength = configuration.LengthFieldLength;
                       pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                            configuration.MaxFrameLength, 0, lengthFieldLength, -1 * lengthFieldLength, 0, true));
                       pipeline.AddLast(decoder, encoder, handler);
                   }));
                IChannel bootstrapChannel = await bootstrap.BindAsync(configuration.IPAddress, configuration.Port);
                logger.LogInformation($"Server start at {configuration.Ip}:{configuration.Port}.");
                await stopFunc();
                await bootstrapChannel.CloseAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
            finally
            {
                var quietPeriod = configuration.QuietPeriodTimeSpan;
                var shutdownTimeout = configuration.ShutdownTimeoutTimeSpan;
                await workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
                await bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
            }
        }
    }
}