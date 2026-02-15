using System.Text.Json.Serialization;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class TargetCreateTargetRequest
    {
        public string Url { get; set; }

        public object BrowserContextId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Background { get; set; }
    }
}
