using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tars.Csharp.Hosting.Configurations;

namespace Tars.Csharp.Hosting.Tcp
{
    public class LibuvTcpServerHost : IServerHost
    {
        public IServiceProvider Provider { get; }

        private readonly HostConfiguration configuration;
        private readonly ILogger<LibuvTcpServerHost> logger;
        private DispatcherEventLoopGroup bossGroup;
        private WorkerEventLoopGroup workerGroup;

        public LibuvTcpServerHost(IServiceProvider provider, HostConfiguration configuration, ILogger<LibuvTcpServerHost> logger)
        {
            Provider = provider;
            this.configuration = configuration;
            this.logger = logger;
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
                       //pipeline.AddLast(new HttpServerCodec());
                       //pipeline.AddLast(new HttpObjectAggregator(65536));
                       //pipeline.AddLast(new WebSocketServerHandler());
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