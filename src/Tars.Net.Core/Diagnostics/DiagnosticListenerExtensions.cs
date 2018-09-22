using System;
using System.Diagnostics;
using Tars.Net.Metadata;

namespace Tars.Net.Diagnostics
{
    public static class DiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "Tars.Net";

        public const string DiagnosticHostingRequest = "Tars.Net.Hosting.Request";
        public const string DiagnosticHostingResponse = "Tars.Net.Hosting.Response";
        public const string DiagnosticHostingException = "Tars.Net.Hosting.Exception";
        public const string DiagnosticClientRequest = "Tars.Net.Client.Request";
        public const string DiagnosticClientResponse = "Tars.Net.Client.Response";
        public const string DiagnosticClientException = "Tars.Net.Client.Exception";

        public static void HostingRequest(this DiagnosticListener listener, Request request)
        {
            if (listener.IsEnabled(DiagnosticHostingRequest))
            {
                listener.Write(DiagnosticHostingRequest, new
                {
                    Request = request
                });
            }
        }

        public static void HostingResponse(this DiagnosticListener listener, Request request, Response response)
        {
            if (listener.IsEnabled(DiagnosticHostingResponse))
            {
                listener.Write(DiagnosticHostingResponse, new
                {
                    Request = request,
                    Response = response
                });
            }
        }

        public static void HostingException(this DiagnosticListener listener, Request request, Response response, Exception exception)
        {
            if (listener.IsEnabled(DiagnosticHostingException))
            {
                listener.Write(DiagnosticHostingException, new
                {
                    Request = request,
                    Response = response,
                    Exception = exception
                });
            }
        }

        public static void ClientRequest(this DiagnosticListener listener, Request request)
        {
            if (listener.IsEnabled(DiagnosticClientRequest))
            {
                listener.Write(DiagnosticClientRequest, new
                {
                    Request = request
                });
            }
        }

        public static void ClientResponse(this DiagnosticListener listener, Request request, Response response)
        {
            if (listener.IsEnabled(DiagnosticClientResponse))
            {
                listener.Write(DiagnosticClientResponse, new
                {
                    Request = request,
                    Response = response
                });
            }
        }

        public static void ClientException(this DiagnosticListener listener, Request request, Response response, Exception exception)
        {
            if (listener.IsEnabled(DiagnosticClientException))
            {
                listener.Write(DiagnosticClientException, new
                {
                    Request = request,
                    Response = response,
                    Exception = exception
                });
            }
        }
    }
}