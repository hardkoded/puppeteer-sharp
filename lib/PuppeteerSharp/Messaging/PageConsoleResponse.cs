using Newtonsoft.Json.Linq;

namespace CefSharp.Puppeteer.Messaging
{
    internal class PageConsoleResponse
    {
        public ConsoleType Type { get; set; }

        public RemoteObject[] Args { get; set; }

        public int ExecutionContextId { get; set; }

        public StackTrace StackTrace { get; set; }
    }
}
