namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ContinueWithAuthRequestChallengeResponse
    {
        public string Response { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
