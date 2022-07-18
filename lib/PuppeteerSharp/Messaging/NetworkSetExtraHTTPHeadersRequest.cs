using System.Collections.Generic;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class NetworkSetExtraHTTPHeadersRequest
    {
        public Dictionary<string, string> Headers { get; set; }
    }
}
