namespace PuppeteerSharp.Messaging
{
    internal class FetchEnableRequest
    {
        public bool HandleAuthRequests { get; set; }
        public Pattern[] Patterns { get; set; }

        internal class Pattern
        {
            public string UrlPattern { get; set; }
        }
    }
}
