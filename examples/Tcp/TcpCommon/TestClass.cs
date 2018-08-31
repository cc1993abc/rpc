using AspectCore.Extensions.Reflection;
using DotNetty.Buffers;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace TcpCommon
{
    public class TestDecoder : IDecoder<IByteBuffer>
    {
        public Request DecodeRequest(IByteBuffer input)
        {
            var result =
                    JsonConvert.DeserializeObject<Request>(input.ReadString(input.ReadableBytes, Encoding.UTF8));
            input.MarkReaderIndex();
            return result;
        }

        public Response DecodeResponse(IByteBuffer input)
        {
            var result = JsonConvert.DeserializeObject<Response>(input.ReadString(input.ReadableBytes, Encoding.UTF8));
            input.MarkReaderIndex();
            return result;
        }
    }

    public class TestContentDecoder : IContentDecoder
    {
        public void DecodeRequestContent(Request req)
        {
            for (int i = 0; i < req.ParameterTypes.Length; i++)
            {
                if (req.Parameters[i] == null || !req.ParameterTypes[i].ParameterType.IsValueType)
                {
                    continue;
                }
                req.Parameters[i] = Convert.ChangeType(req.Parameters[i], req.ParameterTypes[i].ParameterType);
            }
        }

        private static readonly MethodInfo FromResult = typeof(Task).GetTypeInfo().GetMethod(nameof(Task.FromResult));

        public void DecodeResponseContent(Response resp)
        {
            for (int i = 0; i < resp.ReturnParameterTypes.Length; i++)
            {
                if (resp.ReturnParameterTypes[i] == null || !resp.ReturnParameterTypes[i].ParameterType.IsValueType)
                {
                    continue;
                }
                resp.ReturnParameters[i] = Convert.ChangeType(resp.ReturnParameters[i], resp.ReturnParameterTypes[i].ParameterType);
            }
            var type = resp.ReturnValueType.ParameterType;
            var info = type.GetTypeInfo();
            if (info.IsTask())
            {
                resp.ReturnValue = Task.CompletedTask;
            }
            else if (info.IsTaskWithResult())
            {
                resp.ReturnValue = FromResult.MakeGenericMethod(type.GetGenericArguments()).GetReflector().StaticInvoke(resp.ReturnValue);
            }
            else if (info.IsValueTask())
            {
                resp.ReturnValue = Activator.CreateInstance(type, resp.ReturnValue);
            }
            else if (resp.ReturnValue != null || !type.IsValueType)
            {
                resp.ReturnValue = Convert.ChangeType(resp.ReturnValue, resp.ReturnValueType.ParameterType);
            }
        }
    }

    public class TestEncoder : IEncoder<IByteBuffer>
    {
        public IByteBuffer EncodeRequest(Request req)
        {
            req.ParameterTypes = null;
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req)));
        }

        public IByteBuffer EncodeResponse(Response message)
        {
            if (message.ReturnValueType != null)
            {
                var type = message.ReturnValueType.ParameterType;
                var info = type.GetTypeInfo();
                if (info.IsTask() || type == typeof(void))
                {
                    message.ReturnValue = null;
                }
                else if (message.ReturnValue != null && (info.IsValueTask() || info.IsTaskWithResult()))
                {
                    message.ReturnValue = type.GetProperty("Result").GetReflector().GetValue(message.ReturnValue);
                }
                message.ReturnParameterTypes = null;
                message.ReturnValueType = null;
            }
            return Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}