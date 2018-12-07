namespace PuppeteerSharp.Messaging
{
    internal class ResponseReceivedResponse
    {
        public string RequestId { get; set; }
        public ResponsePayload Response { get; set; }
    }
}
