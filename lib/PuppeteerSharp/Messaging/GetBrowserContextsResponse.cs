using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetBrowserContextsResponse
    {
        [JsonProperty("browserContextIds")]
        public string[] BrowserContextIds { get; set; }
    }
}
