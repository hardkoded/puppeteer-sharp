namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RuntimeEvaluateRequest
    {
        public string Expression { get; set; }

        public bool AwaitPromise { get; set; }

        public bool ReturnByValue { get; set; }
    }
}
