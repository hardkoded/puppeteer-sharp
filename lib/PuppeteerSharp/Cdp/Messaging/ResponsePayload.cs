using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ResponsePayload
    {
        public string Url { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public HttpStatusCode Status { get; set; }

        public SecurityDetails SecurityDetails { get; set; }

        public bool FromDiskCache { get; set; }

        public bool FromServiceWorker { get; set; }

        public string StatusText { get; set; }

        [JsonPropertyName("remoteIPAddress")]
        public string RemoteIPAddress { get; set; }

        public int RemotePort { get; set; }
    }
}
