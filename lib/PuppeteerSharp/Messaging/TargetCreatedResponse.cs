using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class TargetCreatedResponse
    {
        [JsonProperty("targetInfo")]
        public TargetInfo TargetInfo { get; set; }
    }
}