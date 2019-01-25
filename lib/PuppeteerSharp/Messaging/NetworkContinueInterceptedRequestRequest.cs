using System.Collections.Generic;
namespace PuppeteerSharp.Messaging
{
    internal class NetworkContinueInterceptedRequestRequest
    {
        public string InterceptionId { get; set; }
        public NetworkContinueInterceptedRequestChallengeResponse AuthChallengeResponse { get; set; }
        public string RawResponse { get; set; }
        public string ErrorReason { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string PostData { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
