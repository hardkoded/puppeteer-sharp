using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LoadingFinishedResponse
    {
        [JsonProperty(Constants.REQUEST_ID)]
        public string RequestId { get; set; }
    }
}
