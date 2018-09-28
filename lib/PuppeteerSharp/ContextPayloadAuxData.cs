using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class ContextPayloadAuxData
    {
        [JsonProperty(Constants.FRAME_ID)]
        internal string FrameId { get; set; }
        [JsonProperty(Constants.IS_DEFAULT)]
        internal bool IsDefault { get; set; }
    }
}