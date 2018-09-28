using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class DebuggerScriptParsedResponse
    {
        [JsonProperty(Constants.URL)]
        public string Url { get; set; }

        [JsonProperty(Constants.SCRIPT_ID)]
        public string ScriptId { get; set; }
    }
}