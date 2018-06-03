using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal struct ContextPayloadAuxData
    {
        [JsonProperty("frameId")]
        internal string FrameId { get; set; }
        [JsonProperty("isDefault")]
        internal bool IsDefault { get; set; }
    }
}