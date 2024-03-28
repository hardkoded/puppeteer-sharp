namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkSetUserAgentOverrideRequest
    {
        public string UserAgent { get; set; }

        public UserAgentMetadata UserAgentMetadata { get; set; }
    }
}
