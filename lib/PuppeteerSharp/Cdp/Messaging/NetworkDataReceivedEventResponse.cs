namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkDataReceivedEventResponse
    {
        public string RequestId { get; set; }

        /// <summary>
        /// Base64-encoded data.
        /// </summary>
        public string Data { get; set; }
    }
}
