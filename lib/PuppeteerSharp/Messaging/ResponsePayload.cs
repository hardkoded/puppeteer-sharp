using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class ResponsePayload
    {
        public string Url { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public HttpStatusCode Status { get; set; }
        public SecurityDetails SecurityDetails { get; set; }
        public bool FromDiskCache { get; set; }
        public bool FromServiceWorker { get; set; }
        public string StatusText { get; set; }
        [JsonProperty("remoteIPAddress")]
        public string RemoteIPAddress { get; set; }
        public int RemotePort { get; set; }
    }
}