using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace PuppeteerSharp.Messaging
{
    internal class FetchFulfillRequest
    {
        public string RequestId { get; set; }
        public int ResponseCode { get; set; }
        public string ResponsePhrase => ReasonPhrases.GetReasonPhrase(ResponseCode);
        public Header[] ResponseHeaders { get; set; }
        public string Body { get; set; }
    }
}
