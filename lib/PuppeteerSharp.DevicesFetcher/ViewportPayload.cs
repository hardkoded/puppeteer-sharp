using Newtonsoft.Json;

namespace PuppeteerSharp.DevicesFetcher
{
    public class ViewportPayload
    {
        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }
    }
}