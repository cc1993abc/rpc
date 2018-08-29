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
        public Request DecodeRequest(dynamic input)
        {
            if (input is IByteBuffer byteBuffer)
            {
                var result =
                    JsonConvert.DeserializeObject<Request>(byteBuffer.ReadString(byteBuffer.ReadableBytes, Encoding.UTF8));
                byteBuffer.MarkReaderIndex();
                return result;
            }

            return null;
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

        public Response DecodeResponse(dynamic input)
        {
            if (input is IByteBuffer byteBuffer)
            {
                var result = JsonConvert.DeserializeObject<Response>(byteBuffer.ReadString(byteBuffer.ReadableBytes, Encoding.UTF8));
                byteBuffer.MarkReaderIndex();
                return result;
            }
            return null;
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
        public object EncodeRequest(Request req)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)));
        }

        public object EncodeResponse(Response message)
        {
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}