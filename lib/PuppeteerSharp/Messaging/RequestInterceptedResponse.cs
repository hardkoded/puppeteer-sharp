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
        public Dictionary<string, object> ResponseHeaders { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string RedirectUrl { get; set; }
        public AuthChallengeData AuthChallenge { get; set; }
        public string RequestId { get; set; }

        internal class AuthChallengeData
        {
            public string Source { get; set; }
            public string Origin { get; set; }
            public string Scheme { get; set; }
            public string Realm { get; set; }
        }
    }
}