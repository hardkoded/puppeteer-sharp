using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class TargetDestroyedResponse
    {
        [JsonProperty(Constants.TARGET_ID)]
        public string TargetId { get; set; }
    }
}