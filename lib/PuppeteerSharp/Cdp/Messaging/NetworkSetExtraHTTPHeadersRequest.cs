using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkSetExtraHTTPHeadersRequest(Dictionary<string, string> headers)
    {
        public Dictionary<string, string> Headers { get; set; } = headers;
    }
}
