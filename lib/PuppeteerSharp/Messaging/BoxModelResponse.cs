using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class BoxModelResponse
    {
        [JsonProperty("model")]
        public BoxModelResponseModel Model { get; set; }

        public class BoxModelResponseModel
        {
            [JsonProperty("content")]
            public decimal[] Content { get; set; }

            [JsonProperty("padding")]
            public decimal[] Padding { get; set; }

            [JsonProperty("border")]
            public decimal[] Border { get; set; }

            [JsonProperty("margin")]
            public decimal[] Margin { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }
        }
    }
}
