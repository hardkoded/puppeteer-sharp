using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class PageExtensions
    {
        public static IEnumerable<IFrame> ChildFrames(this IPage page) => page.Frames.Where(f => f.ParentFrame == page.MainFrame);

        public static async Task<IFrame> FirstChildFrameAsync(this IPage page, int timeoutMs = 5_000)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                var frame = page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
                if (frame != null)
                {
                    return frame;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }

            return null;
        }
    }
}
