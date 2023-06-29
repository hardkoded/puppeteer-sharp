namespace PuppeteerSharp.Messaging
{
    internal class RuntimeAddBindingRequest
    {
        public string Name { get; set; }

        public string ExecutionContextName { get; set; }

        public int? ExecutionContextId { get; set; }
    }
}
