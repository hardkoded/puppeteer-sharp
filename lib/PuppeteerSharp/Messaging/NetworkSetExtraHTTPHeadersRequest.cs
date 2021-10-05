using System.Collections.Generic;

namespace CefSharp.Puppeteer.Messaging
{
    internal class NetworkSetExtraHTTPHeadersRequest
    {
        public Dictionary<string, string> Headers { get; set; }
    }
}
