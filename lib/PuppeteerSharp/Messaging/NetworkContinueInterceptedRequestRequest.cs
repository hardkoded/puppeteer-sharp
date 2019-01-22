using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class NetworkContinueInterceptedRequestRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InterceptionId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NetworkContinueInterceptedRequestChallengeResponse AuthChallengeResponse { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RawResponse { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorReason { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PostData { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

    internal class NetworkContinueInterceptedRequestChallengeResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Response { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
    }
}
