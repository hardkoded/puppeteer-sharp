using System;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class Clip
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("scale")]
        public int Scale { get; internal set; }

        internal Clip Clone()
        {
            return new Clip
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height
            };
        }
    }
}