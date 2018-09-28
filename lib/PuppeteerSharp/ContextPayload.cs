using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class ContextPayload
    {
        [JsonProperty(Constants.ID)]
        internal int Id { get; set; }
        [JsonProperty(Constants.AUX_DATA)]
        internal ContextPayloadAuxData AuxData { get; set; }
    }
}