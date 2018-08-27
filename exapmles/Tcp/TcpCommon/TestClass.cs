using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using Tars.Net.Codecs;
using Tars.Net.Metadata;

namespace TcpCommon
{
    public class TestRequestDecoder : RequestDecoder
    {
        public override Request DecodeRequest(IByteBuffer input)
        {
            throw new NotImplementedException();
        }

        public override void DecodeRequestContent(Request req)
        {
            throw new NotImplementedException();
        }
    }

    public class TestRequestEncoder : RequestEncoder
    {
        public override IByteBuffer Encode(Request req)
        {
            throw new NotImplementedException();
        }
    }

    public class TestResponseDecoder : ResponseDecoder
    {
        public override Response DecodeResponse(IByteBuffer input)
        {
            throw new NotImplementedException();
        }

        public override void DecodeResponseContent(Response resp)
        {
            throw new NotImplementedException();
        }
    }

    public class TestResponseEncoder : ResponseEncoder
    {
        public override IByteBuffer EncodeResponse(Response message)
        {
            throw new NotImplementedException();
        }
    }
}
