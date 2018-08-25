using System;
using System.Net;

namespace Tars.Net.Hosting.Configurations
{
    public class HostConfiguration
    {
        public string Ip { get; set; } = "127.0.0.1";

        public IPAddress IPAddress => IPAddress.Parse(Ip);

        public int Port { get; set; } = 8989;

        public int QuietPeriodSeconds { get; set; } = 1;

        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromSeconds(QuietPeriodSeconds);

        public int ShutdownTimeoutSeconds { get; set; } = 3;

        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);

        public int SoBacklog { get; set; } = 8192;

        public int MaxFrameLength { get; set; } = 100 * 1024 * 1024;

        public int LengthFieldLength { get; set; } = 4;
    }
}