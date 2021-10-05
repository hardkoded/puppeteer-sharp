using System.Linq;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests
{
    public static class PageExtensions
    {
        public static Frame FirstChildFrame(this Page page) => page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
    }
}
