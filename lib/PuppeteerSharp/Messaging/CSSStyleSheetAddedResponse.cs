namespace PuppeteerSharp.Messaging
{
    internal class CSSStyleSheetAddedResponse
    {
        public CSSStyleSheetAddedResponseHeader Header { get; set; }

        public class CSSStyleSheetAddedResponseHeader
        {
            public string StyleSheetId { get; set; }
            public string SourceURL { get; set; }
        }
    }
}
