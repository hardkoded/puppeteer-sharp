namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchEnableRequest
    {
        public bool HandleAuthRequests { get; set; }

        public Pattern[] Patterns { get; set; }

        internal class Pattern(string urlPattern)
        {
            public string UrlPattern { get; set; } = urlPattern;
        }
    }
}
