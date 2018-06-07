using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestServedFromCacheResponse
    {
        [JsonProperty("requestId")]
        internal string RequestId { get; set; }
    }
}
