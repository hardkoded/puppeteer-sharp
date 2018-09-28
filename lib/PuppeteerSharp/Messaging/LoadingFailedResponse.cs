using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LoadingFailedResponse
    {
        [JsonProperty(Constants.REQUEST_ID)]
        public string RequestId { get; set; }

        [JsonProperty(Constants.ERROR_TEXT)]
        public string ErrorText { get; set; }        
    }
}
