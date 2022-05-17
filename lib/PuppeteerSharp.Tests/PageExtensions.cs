using System.Linq;

namespace PuppeteerSharp.Tests
{
    public static class PageExtensions
    {
        public static IFrame FirstChildFrame(this Page page) => page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
    }
}
