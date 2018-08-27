using DotNetty.Buffers;
using Newtonsoft.Json;
using System.Text;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace TcpCommon
{
    public class TestRequestDecoder : RequestDecoder
    {
        public override Request DecodeRequest(IByteBuffer input)
        {
            return JsonConvert.DeserializeObject<Request>(input.GetString(0, input.ReadableBytes, Encoding.UTF8));
        }

        public override void DecodeRequestContent(Request req)
        {
        }
    }

    public class TestRequestEncoder : RequestEncoder
    {
        public override IByteBuffer Encode(Request req)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)));
        }
    }

    public class TestResponseDecoder : ResponseDecoder
    {
        public override Response DecodeResponse(IByteBuffer input)
        {
            return JsonConvert.DeserializeObject<Response>(input.GetString(0, input.ReadableBytes, Encoding.UTF8));
        }

        public override void DecodeResponseContent(Response resp)
        {
        }
    }

    public class TestResponseEncoder : ResponseEncoder
    {
        public override IByteBuffer EncodeResponse(Response message)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}