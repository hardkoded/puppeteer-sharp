using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class RequestInterceptedResponse
    {
        public string InterceptionId { get; set; }
        public Payload Request { get; set; }
        public string FrameId { get; set; }
        public ResourceType ResourceType { get; set; }
        public bool IsNavigationRequest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> ResponseHeaders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode ResponseStatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RedirectUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AuthChallengeData AuthChallenge { get; set; }

        internal class AuthChallengeData
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Source { get; set; }
            public string Origin { get; set; }
            public string Scheme { get; set; }
            public string Realm { get; set; }
        }
    }
}
