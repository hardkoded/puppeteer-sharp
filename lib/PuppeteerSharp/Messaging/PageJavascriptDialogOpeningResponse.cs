using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageJavascriptDialogOpeningResponse
    {
        [JsonProperty("type")]
        internal DialogType Type { get; set; }

        [JsonProperty("defaultPrompt")]
        internal string DefaultPrompt { get; set; }

        [JsonProperty("message")]
        internal string Message { get; set; }
    }
}
