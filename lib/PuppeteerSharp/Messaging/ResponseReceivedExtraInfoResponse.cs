using System.Collections.Generic;
using System.Net;

namespace CefSharp.Puppeteer.Messaging
{
    internal class ResponseReceivedExtraInfoResponse
    {
        public string RequestId { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string HeadersText { get; set; }
    }
}
