using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;

namespace Tars.Net.Hosting.Tcp
{
    public class LibuvTcpServerHost : IServerHost
    {
        public IServiceProvider Provider { get; }

        private readonly RpcConfiguration configuration;
        private readonly ILogger<LibuvTcpServerHost> logger;
        private readonly IDecoder decoder;
        private readonly IEncoder encoder;
        private readonly ServerHandlerBase handler;
        private DispatcherEventLoopGroup bossGroup;
        private WorkerEventLoopGroup workerGroup;

        public LibuvTcpServerHost(IServiceProvider provider, RpcConfiguration configuration,
            ILogger<LibuvTcpServerHost> logger, IDecoder decoder, IEncoder encoder,
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
                            configuration.MaxFrameLength, 0, lengthFieldLength, 0, lengthFieldLength, true));
                       pipeline.AddLast(new RequestDecoder(decoder), new LengthFieldPrepender(lengthFieldLength), new ResponseEncoder(encoder), handler);
                   }));
                IChannel bootstrapChannel = await bootstrap.BindAsync(configuration.Port);
                logger.LogInformation($"Server start at {IPAddress.Any}:{configuration.Port}.");
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
                foreach (var item in Provider.GetServices<IRpcClient>())
                {
                    await item.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout);
                }
            }
        }
    }
}