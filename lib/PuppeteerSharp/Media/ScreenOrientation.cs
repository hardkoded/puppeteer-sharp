using Newtonsoft.Json;

namespace PuppeteerSharp.Media
{
    internal class ScreenOrientation
    {
        [JsonProperty("angle")]
        public int Angle { get; internal set; }
        [JsonProperty("type")]
        public string Type { get; internal set; }
    }
}