using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class BoxModelResponse
    {
        [JsonProperty(Constants.MODEL)]
        public BoxModelResponseModel Model { get; set; }

        public class BoxModelResponseModel
        {
            [JsonProperty(Constants.CONTENT)]
            public decimal[] Content { get; set; }

            [JsonProperty(Constants.PADDING)]
            public decimal[] Padding { get; set; }

            [JsonProperty(Constants.BORDER)]
            public decimal[] Border { get; set; }

            [JsonProperty(Constants.MARGIN)]
            public decimal[] Margin { get; set; }

            [JsonProperty(Constants.WIDTH)]
            public int Width { get; set; }

            [JsonProperty(Constants.HEIGHT)]
            public int Height { get; set; }
        }
    }
}
