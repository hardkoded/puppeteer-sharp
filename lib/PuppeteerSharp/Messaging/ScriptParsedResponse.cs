using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class ScriptParsedResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("scriptId")]
        public string ScriptId { get; set; }
    }
}