namespace PuppeteerSharp.Cdp.Messaging
{
    internal class FetchContinueRequestRequest
    {
        public string RequestId { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string PostData { get; set; }

        public Header[] Headers { get; set; }
    }
}
