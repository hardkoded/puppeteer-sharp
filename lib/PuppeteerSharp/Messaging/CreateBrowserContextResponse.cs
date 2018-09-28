using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CreateBrowserContextResponse
    {
        [JsonProperty(Constants.BROWSER_CONTEXT_ID)]
        public string BrowserContextId { get; set; }
    }
}
