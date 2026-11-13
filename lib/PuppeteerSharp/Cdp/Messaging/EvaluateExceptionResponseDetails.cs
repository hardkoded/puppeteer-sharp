namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EvaluateExceptionResponseDetails
    {
        public EvaluateExceptionResponseInfo Exception { get; set; }

        public string Text { get; set; }

        public EvaluateExceptionResponseStackTrace StackTrace { get; set; }

        public int? ExecutionContextId { get; set; }

        public string Url { get; set; }

        public int? LineNumber { get; set; }

        public int? ColumnNumber { get; set; }
    }
}
