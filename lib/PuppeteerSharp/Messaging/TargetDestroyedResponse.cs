using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class TargetDestroyedResponse
    {
        [JsonProperty("targetId")]
        public string TargetId { get; set; }
    }
}