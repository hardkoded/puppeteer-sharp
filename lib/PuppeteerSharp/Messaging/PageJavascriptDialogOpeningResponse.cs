using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class PageJavascriptDialogOpeningResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("defaultPrompt")]
        public string DefaultPrompt { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
