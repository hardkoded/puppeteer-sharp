using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetBrowserContextsResponse
    {
        [JsonProperty(Constants.BROWSER_CONTEXT_IDS)]
        public string[] BrowserContextIds { get; set; }
    }
}
