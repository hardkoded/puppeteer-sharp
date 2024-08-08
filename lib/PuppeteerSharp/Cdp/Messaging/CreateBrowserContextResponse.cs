using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class CreateBrowserContextResponse
    {
        [JsonConverter(typeof(AnyTypeToStringConverter))]
        public string BrowserContextId { get; set; }
    }
}
