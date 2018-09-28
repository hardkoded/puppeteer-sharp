using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class RequestInterceptedResponse
    {
        [JsonProperty(Constants.INTERCEPTION_ID)]
        public string InterceptionId { get; set; }
        
        [JsonProperty(Constants.REQUEST)]
        public Payload Request { get; set; }

        [JsonProperty(Constants.FRAME_ID)]
        public string FrameId { get; set; }

        [JsonProperty(Constants.RESOURCE_TYPE)]
        public ResourceType ResourceType { get; set; }

        [JsonProperty(Constants.IS_NAVIGATION_REQUEST)]
        public bool IsNavigationRequest { get; set; }
        
        [JsonProperty(Constants.RESPONSE_HEADERS, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> ResponseHeaders { get; set; }

        [JsonProperty(Constants.RESPONSE_STATUS_CODE, NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode ResponseStatusCode { get; set; }

        [JsonProperty(Constants.REDIRECT_URL, NullValueHandling = NullValueHandling.Ignore)]
        public string RedirectUrl { get; set; }

        [JsonProperty(Constants.AUTH_CHALLENGE, NullValueHandling = NullValueHandling.Ignore)]
        public AuthChallengeData AuthChallenge { get; set; }

        internal class AuthChallengeData
        {
            [JsonProperty(Constants.SOURCE, NullValueHandling = NullValueHandling.Ignore)]
            public string Source { get; set; }

            [JsonProperty(Constants.ORIGIN)]
            public string Origin { get; set; }

            [JsonProperty(Constants.SCHEME)]
            public string Scheme { get; set; }

            [JsonProperty(Constants.REALM)]
            public string Realm { get; set; }
        }
    }
}
