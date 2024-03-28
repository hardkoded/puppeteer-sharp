namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EvaluateHandleResponse
    {
        public EvaluateExceptionResponseDetails ExceptionDetails { get; set; }

        public RemoteObject Result { get; set; }
    }
}
