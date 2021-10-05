namespace CefSharp.Puppeteer.Messaging
{
    internal class LoadingFailedResponse
    {
        public string RequestId { get; set; }

        public string ErrorText { get; set; }
    }
}
