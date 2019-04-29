using System.Collections.Generic;
namespace PuppeteerSharp.Messaging
{
    internal class FetchFulfillRequest
    {
        public string RequestId { get; set; }
        public int ResponseCode { get; set; }
        public Header[] ResponseHeaders { get; set; }
        public string Body { get; set; }
    }
}
