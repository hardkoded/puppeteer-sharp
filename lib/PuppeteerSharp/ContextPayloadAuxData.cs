using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class ContextPayloadAuxData
    {
        [JsonProperty("frameId")]
        internal string FrameId { get; set; }
        [JsonProperty("isDefault")]
        internal bool IsDefault { get; set; }
    }
}