using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class RequestServedFromCacheResponse
    {
        [JsonProperty(Constants.REQUEST_ID)]
        internal string RequestId { get; set; }
    }
}
