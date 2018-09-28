using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageJavascriptDialogOpeningResponse
    {
        [JsonProperty(Constants.TYPE)]
        internal DialogType Type { get; set; }

        [JsonProperty(Constants.DEFAULT_PROMPT)]
        internal string DefaultPrompt { get; set; }

        [JsonProperty(Constants.MESSAGE)]
        internal string Message { get; set; }
    }
}
