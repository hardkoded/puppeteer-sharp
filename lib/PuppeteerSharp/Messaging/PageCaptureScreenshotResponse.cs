using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageCaptureScreenshotResponse
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
