using Tars.Net.Metadata;
using Xunit;

namespace Tars.Net.UT.Abstractions.Metadata
{
    public class RequestTest
    {
        private readonly Request sut;

        public RequestTest()
        {
            sut = new Request()
            {
                Version = 1,
                MessageType = 4,
                RequestId = 2,
                ServantName = "test",
                FuncName = "func",
                Timeout = 3,
                PacketType = 6,
                Buffer = null,
                Parameters = null,
                Codec = Codecs.Codec.Tars,
                IsOneway = false,
                ParameterTypes = null
            };
        }

        [Fact]
        public void CreateResponseShouldBeCopySameValues()
        {
            var resp = sut.CreateResponse();
            Assert.Equal(sut.Version, resp.Version);
            Assert.Equal(sut.MessageType, resp.MessageType);
            Assert.Equal(sut.RequestId, resp.RequestId);
            Assert.Equal(sut.ServantName, resp.ServantName);
            Assert.Equal(sut.FuncName, resp.FuncName);
            Assert.Equal(sut.Timeout, resp.Timeout);
        }
    }
}