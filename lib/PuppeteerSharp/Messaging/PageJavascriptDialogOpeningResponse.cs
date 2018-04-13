using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class PageJavascriptDialogOpeningResponse
    {
        [JsonProperty("type")]
        public DialogType Type { get; set; }
        
        [JsonProperty("defaultPrompt")]
        public string DefaultPrompt { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
