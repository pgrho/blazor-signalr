using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Shipwreck.BlazorSignalR
{
    public sealed class BlazorSignalRException : Exception
    {
        internal BlazorSignalRException(JObject obj)
            : base(obj.Property("stack")?.Value?.Value<string>() ?? "An error caught during connecting to SignalR server.")
        {
            ReadyState = obj.Property("readyState")?.Value?.Value<int?>();
            Status = obj.Property("status")?.Value?.Value<HttpStatusCode?>();
            StatusText = obj.Property("statusText")?.Value?.Value<string>();

            Name = obj.Property("name")?.Value?.Value<string>();
            Stack = obj.Property("stack")?.Value?.Value<string>();
            Transport = obj.Property("transport")?.Value?.Value<string>();
        }

        public int? ReadyState { get; set; }
        public HttpStatusCode? Status { get; set; }
        public string StatusText { get; }

        public string Name { get; }

        public string Stack { get; }

        public string Transport { get; }
    }
}
