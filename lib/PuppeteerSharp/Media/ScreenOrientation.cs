using Newtonsoft.Json;

namespace PuppeteerSharp.Media
{
    internal class ScreenOrientation
    {
        [JsonProperty(Constants.ANGLE)]
        public int Angle { get; internal set; }
        [JsonProperty(Constants.TYPE)]
        public string Type { get; internal set; }
    }
}