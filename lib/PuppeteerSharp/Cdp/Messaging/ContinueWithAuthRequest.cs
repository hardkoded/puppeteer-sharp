using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ContinueWithAuthRequest
    {
        public string RequestId { get; set; }

        public ContinueWithAuthRequestChallengeResponse AuthChallengeResponse { get; set; }

        public string RawResponse { get; set; }

        public string ErrorReason { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string PostData { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
