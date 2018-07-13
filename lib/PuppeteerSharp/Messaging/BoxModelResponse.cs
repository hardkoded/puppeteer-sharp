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
            public int[] Content { get; set; }

            [JsonProperty("padding")]
            public int[] Padding { get; set; }

            [JsonProperty("border")]
            public int[] Border { get; set; }

            [JsonProperty("margin")]
            public int[] Margin { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }
        }
    }
}
