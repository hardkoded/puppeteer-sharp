using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class ResponsePayload
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; set; }
        [JsonProperty("status")]
        public HttpStatusCode Status { get; set; }
        [JsonProperty("securityDetails")]
        public SecurityDetails SecurityDetails { get; set; }
        [JsonProperty("fromDiskCache")]
        public bool FromDiskCache { get; set; }
        [JsonProperty("fromServiceWorker")]
        public bool FromServiceWorker { get; set; }
        [JsonProperty("statusText")]
        public string StatusText { get; set; }
    }
}