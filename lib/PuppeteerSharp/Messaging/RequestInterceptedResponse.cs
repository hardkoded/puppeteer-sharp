using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class RequestInterceptedResponse
    {
        [JsonProperty("interceptionId")]
        public string InterceptionId { get; set; }
        
        [JsonProperty("request")]
        public Payload Request { get; set; }

        [JsonProperty("frameId")]
        public string FrameId { get; set; }

        [JsonProperty("resourceType")]
        public ResourceType ResourceType { get; set; }

        [JsonProperty("isNavigationRequest")]
        public bool IsNavigationRequest { get; set; }
        
        [JsonProperty("responseHeaders", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> ResponseHeaders { get; set; }

        [JsonProperty("responseStatusCode", NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode ResponseStatusCode { get; set; }

        [JsonProperty("redirectUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string RedirectUrl { get; set; }

        [JsonProperty("authChallenge", NullValueHandling = NullValueHandling.Ignore)]
        public AuthChallengeData AuthChallenge { get; set; }

        internal class AuthChallengeData
        {
            [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
            public string Source { get; set; }

            [JsonProperty("origin")]
            public string Origin { get; set; }

            [JsonProperty("scheme")]
            public string Scheme { get; set; }

            [JsonProperty("realm")]
            public string Realm { get; set; }
        }
    }
}
