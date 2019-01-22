namespace PuppeteerSharp.Messaging
{
    internal class NetworkContinueInterceptedRequestRequest
    {
        public string InterceptionId { get; set; }
        public NetworkContinueInterceptedRequestChallengeResponse AuthChallengeResponse { get; set; }
    }

    internal class NetworkContinueInterceptedRequestChallengeResponse
    {
        public string Response { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
