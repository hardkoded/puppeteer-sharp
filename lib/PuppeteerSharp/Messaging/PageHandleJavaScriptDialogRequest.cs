using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageHandleJavaScriptDialogRequest
    {
        public bool Accept { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PromptText { get; set; }
    }
}
