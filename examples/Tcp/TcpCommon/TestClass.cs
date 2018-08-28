using DotNetty.Buffers;
using Newtonsoft.Json;
using System;
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
            for (int i = 0; i < req.ParameterTypes.Length; i++)
            {
                if (req.Parameters[i] == null)
                {
                    continue;
                }
                req.Parameters[i] = Convert.ChangeType(req.Parameters[i], req.ParameterTypes[i].ParameterType);
            }
        }

        public Response DecodeResponse(IByteBuffer input)
        {
            var result = JsonConvert.DeserializeObject<Response>(input.ReadString(input.ReadableBytes, Encoding.UTF8));
            input.MarkReaderIndex();
            return result;
        }

        public void DecodeResponseContent(Response resp)
        {
            for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
            {
                if (resp.ReturnParameterTypes[i] == null)
                {
                    continue;
                }
                resp.ReturnParameters[i] = Convert.ChangeType(resp.ReturnParameters[i], resp.ReturnParameterTypes[i].ParameterType);
            }

            if (resp.ReturnValue != null)
            {
                resp.ReturnValue = Convert.ChangeType(resp.ReturnValue, resp.ReturnValueType.ParameterType);
            }
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