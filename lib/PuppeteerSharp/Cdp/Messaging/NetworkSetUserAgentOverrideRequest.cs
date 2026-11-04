namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkSetUserAgentOverrideRequest
    {
        public string UserAgent { get; set; }

        public string AcceptLanguage { get; set; }

        public UserAgentMetadata UserAgentMetadata { get; set; }

        public string Platform { get; set; }
    }
}
