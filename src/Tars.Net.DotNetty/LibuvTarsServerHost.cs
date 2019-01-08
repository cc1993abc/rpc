using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tars.Net.Configurations;
using Tars.Net.Hosting.Tcp;
using Tars.Net.Hosting.Udp;

namespace Tars.Net.UT.DotNetty
{
    public class LibuvTarsServerHost : IHostedService
    {
        private readonly Dictionary<string, IHostedService> hostServices = new Dictionary<string, IHostedService>();

        public IServiceProvider Provider { get; }
        private readonly ConfigurationManager configurationManager;
        private readonly ILogger<LibuvTarsServerHost> logger;

        public LibuvTarsServerHost(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            Dictionary<string, ServantAdapterConfig> servantAdapters = configurationManager.ServerConfig.ServantAdapters;
            foreach (var servantAdapter in servantAdapters)
            {
                ServantAdapterConfig item = servantAdapter.Value;
                if (item.Endpoint.Proto == "tcp")
                {
                    LibuvTcpServerHost host = new LibuvTcpServerHost(Provider, item);
                    hostServices.Add(servantAdapter.Key, host);
                }
                else if (item.Endpoint.Proto == "udp")
                {
                    UdpServerHost host = new UdpServerHost(Provider, item);
                    hostServices.Add(servantAdapter.Key, host);
                }
            } 
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var item in hostServices)
            {
                await item.Value.StartAsync(cancellationToken);
                logger.LogInformation($"Servant {item.Key} started.");
            } 
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var item in hostServices)
            {
                await item.Value.StopAsync(cancellationToken);
                logger.LogInformation($"Servant {item.Key} stopped.");
            }
        }
    }
}
