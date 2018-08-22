using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CreateBrowserContextResponse
    {
        [JsonProperty("browserContextId")]
        public string BrowserContextId { get; set; }
    }
}
