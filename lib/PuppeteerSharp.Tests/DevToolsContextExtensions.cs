using System.Linq;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests
{
    public static class DevToolsContextExtensions
    {
        public static Frame FirstChildFrame(this DevToolsContext devToolsContext) => devToolsContext.Frames.FirstOrDefault(f => f.ParentFrame == devToolsContext.MainFrame);
    }
}
