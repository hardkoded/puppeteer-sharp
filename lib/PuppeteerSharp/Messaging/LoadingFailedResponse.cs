using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LoadingFailedResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("errorText")]
        public string ErrorText { get; set; }        
    }
}
