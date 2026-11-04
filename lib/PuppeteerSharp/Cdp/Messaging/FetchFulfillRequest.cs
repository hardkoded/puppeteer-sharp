using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchFulfillRequest
    {
        public string RequestId { get; set; }

        public int ResponseCode { get; set; }

        public string ResponsePhrase => HttpStatusTextHelper.GetStatusText(ResponseCode);

        public Header[] ResponseHeaders { get; set; }

        public string Body { get; set; }
    }
}
