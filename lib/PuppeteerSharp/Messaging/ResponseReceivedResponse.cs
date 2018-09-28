using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class ResponseReceivedResponse
    {
        [JsonProperty(Constants.REQUEST_ID)]
        public string RequestId { get; set; }

        [JsonProperty(Constants.RESPONSE)]
        public ResponsePayload Response { get; set; }
    }
}
