using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Diagnostics;
using Tars.Net.Diagnostics;
using Tars.Net.Metadata;
using Xunit;
using D = Tars.Net.Diagnostics.DiagnosticListenerExtensions;

namespace Tars.Net.UT.Core.Diagnostics
{
    public class TestDiagnostic
    {
        public Request Request { get; set; }

        public Response Response { get; set; }

        public Exception Exception { get; set; }

        [DiagnosticName(D.DiagnosticHostingRequest)]
        public void HostingRequest(Request request)
        {
            Request = request;
        }

        [DiagnosticName(D.DiagnosticHostingResponse)]
        public void DiagnosticHostingResponse(Request request, Response response)
        {
            Request = request;
            Response = response;
        }

        [DiagnosticName(D.DiagnosticHostingException)]
        public void DiagnosticHostingException(Request request, Response response, Exception exception)
        {
            Request = request;
            Response = response;
            Exception = exception;
        }

        [DiagnosticName(D.DiagnosticClientRequest)]
        public void DiagnosticClientRequest(Request request)
        {
            Request = request;
        }

        [DiagnosticName(D.DiagnosticClientResponse)]
        public void DiagnosticClientResponse(Request request, Response response)
        {
            Request = request;
            Response = response;
        }

        [DiagnosticName(D.DiagnosticClientException)]
        public void DiagnosticClientException(Request request, Response response, Exception exception)
        {
            Request = request;
            Response = response;
            Exception = exception;
        }
    }

    public class DiagnosticListenerExtensionsTest
    {
        [Fact]
        public void WhenCanHostingRequestShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            listener.HostingRequest(result);
            Assert.Same(result, sut.Request);
        }

        [Fact]
        public void WhenCanHostingResponseShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            var response = new Response();
            listener.HostingResponse(result, response);
            Assert.Same(result, sut.Request);
            Assert.Same(response, sut.Response);
        }

        [Fact]
        public void WhenCanHostingExceptionShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            var response = new Response();
            var ex = new Exception();
            listener.HostingException(result, response, ex);
            Assert.Same(result, sut.Request);
            Assert.Same(response, sut.Response);
            Assert.Same(ex, sut.Exception);
        }

        [Fact]
        public void WhenCanClientRequestShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            listener.ClientRequest(result);
            Assert.Same(result, sut.Request);
        }

        [Fact]
        public void WhenCanClientResponseShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            var response = new Response();
            listener.ClientResponse(result, response);
            Assert.Same(result, sut.Request);
            Assert.Same(response, sut.Response);
        }

        [Fact]
        public void WhenCanClientExceptionShouldSameRequest()
        {
            var listener = new DiagnosticListener(D.DiagnosticListenerName);
            var sut = new TestDiagnostic();
            listener.SubscribeWithAdapter(sut);
            var result = new Request();
            var response = new Response();
            var ex = new Exception();
            listener.ClientException(result, response, ex);
            Assert.Same(result, sut.Request);
            Assert.Same(response, sut.Response);
            Assert.Same(ex, sut.Exception);
        }
    }
}