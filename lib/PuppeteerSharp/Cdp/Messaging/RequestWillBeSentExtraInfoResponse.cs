using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RequestWillBeSentExtraInfoResponse
    {
        public string RequestId { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
