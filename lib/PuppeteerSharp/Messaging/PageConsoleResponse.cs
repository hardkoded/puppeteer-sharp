using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class PageConsoleResponse
    {
        public ConsoleType Type { get; set; }
        public JToken[] Args { get; set; }
        public int ExecutionContextId { get; set; }
    }
}
