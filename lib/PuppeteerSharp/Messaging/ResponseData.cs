using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class ResponseData
    {
        [JsonProperty("url")]
        public string Url { get; internal set; }
        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; internal set; }
        [JsonProperty("status")]
        public HttpStatusCode Status { get; internal set; }
        [JsonProperty("securityDetails")]
        public SecurityDetails SecurityDetails { get; set; }
        [JsonProperty("fromDiskCache")]
        public bool FromDiskCache { get; set; }
        [JsonProperty("fromServiceWorker")]
        public bool FromServiceWorker { get; set; }
    }
}