namespace PuppeteerSharp.Messaging
{
    internal class PageConsoleResponse
    {
        public ConsoleType Type { get; set; }
        public dynamic[] Args { get; set; }
        public int ExecutionContextId { get; set; }
    }
}
