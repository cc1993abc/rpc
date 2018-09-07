using System;
using Tars.Net.Configurations;
using Xunit;

namespace Tars.Net.UT.Abstractions.Configurations
{
    public class RpcConfigurationTest
    {
        [Fact]
        public void DefaultPortShouldBe8989()
        {
            var result = new RpcConfiguration().Port;
            Assert.Equal(8989, result);
        }

        [Fact]
        public void DefaultQuietPeriodSecondsShouldBe1()
        {
            var config = new RpcConfiguration();
            Assert.Equal(1, config.QuietPeriodSeconds);
            Assert.Equal(TimeSpan.FromSeconds(1), config.QuietPeriodTimeSpan);
        }

        [Fact]
        public void DefaultShutdownTimeoutSecondsShouldBe3()
        {
            var config = new RpcConfiguration();
            Assert.Equal(3, config.ShutdownTimeoutSeconds);
            Assert.Equal(TimeSpan.FromSeconds(3), config.ShutdownTimeoutTimeSpan);
        }

        [Fact]
        public void DefaultSoBacklogShouldBe8192()
        {
            var config = new RpcConfiguration();
            Assert.Equal(8192, config.SoBacklog);
        }

        [Fact]
        public void DefaultMaxFrameLengthShouldBe5242880()
        {
            var config = new RpcConfiguration();
            Assert.Equal(5 * 1024 * 1024, config.MaxFrameLength);
        }

        [Fact]
        public void DefaultLengthFieldLengthShouldBe4()
        {
            var config = new RpcConfiguration();
            Assert.Equal(4, config.LengthFieldLength);
        }
    }
}