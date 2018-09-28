using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class ResponsePayload
    {
        [JsonProperty(Constants.URL)]
        public string Url { get; internal set; }
        [JsonProperty(Constants.HEADERS)]
        public Dictionary<string, object> Headers { get; set; }
        [JsonProperty(Constants.STATUS)]
        public HttpStatusCode Status { get; set; }
        [JsonProperty(Constants.SECURITY_DETAILS)]
        public SecurityDetails SecurityDetails { get; set; }
        [JsonProperty(Constants.FROM_DISK_CACHE)]
        public bool FromDiskCache { get; set; }
        [JsonProperty(Constants.FROM_SERVICE_WORKER)]
        public bool FromServiceWorker { get; set; }
        [JsonProperty("statusText")]
        public string StatusText { get; set; }
    }
}