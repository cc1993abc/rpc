using Tars.Net.Attributes;
using Tars.Net.Codecs;
using Xunit;

namespace Tars.Net.UT.Abstractions.Attributes
{
    public class RpcAttributeTest
    {
        private readonly RpcAttribute sut;

        public RpcAttributeTest()
        {
            sut = new RpcAttribute("Test");
        }

        [Fact]
        public void ServantNameShouldBeTest()
        {
            Assert.Equal("Test", sut.ServantName);
        }

        [Fact]
        public void CodecShouldBeTars()
        {
            Assert.Equal(Codec.Tars, sut.Codec);
        }
    }
}