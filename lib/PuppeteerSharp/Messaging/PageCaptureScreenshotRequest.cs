using Newtonsoft.Json;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Messaging
{
    internal class PageCaptureScreenshotRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Format { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Quality { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Clip Clip { get; set; }
    }
}
