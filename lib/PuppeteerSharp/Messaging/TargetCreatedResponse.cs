using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class TargetCreatedResponse
    {
        [JsonProperty(Constants.TARGET_INFO)]
        public TargetInfo TargetInfo { get; set; }
    }
}