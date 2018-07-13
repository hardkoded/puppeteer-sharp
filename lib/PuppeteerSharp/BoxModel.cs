using Newtonsoft.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents boxes of the element.
    /// </summary>
    public class BoxModel
    {
        internal BoxModel()
        {
        }

        [JsonProperty("content")]
        public Point[] Content { get; internal set; }

        [JsonProperty("padding")]
        public Point[] Padding { get; internal set; }

        [JsonProperty("border")]
        public Point[] Border { get; internal set; }

        [JsonProperty("margin")]
        public Point[] Margin { get; internal set; }

        [JsonProperty("width")]
        public int Width { get; internal set; }

        [JsonProperty("height")]
        public int Height { get; internal set; }
    }

    public struct Point
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }
    }
}