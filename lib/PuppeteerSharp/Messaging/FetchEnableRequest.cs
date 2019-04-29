namespace PuppeteerSharp.Messaging
{
    internal class FetchEnableRequest
    {
        public bool HandleAuthRequests { get; internal set; }
        public Pattern[] Patterns { get; internal set; }

        internal class Pattern
        {
            public string UrlPattern { get; internal set; }
        }
    }
}
