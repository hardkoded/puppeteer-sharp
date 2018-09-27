using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class DebuggerScriptParsedResponse
    {
        [JsonProperty(Constants.URL)]
        public string Url { get; set; }

        [JsonProperty("scriptId")]
        public string ScriptId { get; set; }
    }
}