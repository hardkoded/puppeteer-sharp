using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LoadingFinishedResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }
    }
}
