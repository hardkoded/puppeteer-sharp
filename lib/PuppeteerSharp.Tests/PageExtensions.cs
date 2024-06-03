using System.Collections.Generic;
using System.Linq;

namespace PuppeteerSharp.Tests
{
    public static class PageExtensions
    {
        public static IEnumerable<IFrame> ChildFrames(this IPage page) => page.Frames.Where(f => f.ParentFrame == page.MainFrame);

        public static IFrame FirstChildFrame(this IPage page) => page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
    }
}
