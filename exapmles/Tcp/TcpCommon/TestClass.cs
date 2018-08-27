using DotNetty.Buffers;
using Newtonsoft.Json;
using System.Text;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace TcpCommon
{
    public class TestDecoder : IDecoder
    {
        public Request DecodeRequest(IByteBuffer input)
        {
            var result = JsonConvert.DeserializeObject<Request>(input.ReadString(input.ReadableBytes, Encoding.UTF8));
            input.MarkReaderIndex();
            return result;
        }

        public void DecodeRequestContent(Request req)
        {
        }

        public Response DecodeResponse(IByteBuffer input)
        {
            var result = JsonConvert.DeserializeObject<Response>(input.ReadString(input.ReadableBytes, Encoding.UTF8));
            input.MarkReaderIndex();
            return result;
        }

        public void DecodeResponseContent(Response resp)
        {
        }
    }

    public class TestEncoder : IEncoder
    {
        public IByteBuffer EncodeRequest(Request req)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)));
        }

        public IByteBuffer EncodeResponse(Response message)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}