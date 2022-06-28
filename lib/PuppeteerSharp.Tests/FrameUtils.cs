using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class FrameUtils
    {
        public static Task<Frame> AttachFrameAsync(Page page, string frameId, string url)
            => AttachFrameAsync(page.MainFrame, frameId, url);

        public static async Task<Frame> AttachFrameAsync(Frame frame, string frameId, string url)
        {
            var handle = (ElementHandle)await frame.EvaluateFunctionHandleAsync(@" async (frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              await new Promise(x => frame.onload = x);
              return frame
            }", frameId, url);
            return await handle.ContentFrameAsync();
        }

        public static Task DetachFrameAsync(Page page, string frameId)
            => DetachFrameAsync(page.MainFrame, frameId);

        public static Task DetachFrameAsync(Frame frame, string frameId)
            => frame.EvaluateFunctionAsync(@"function detachFrame(frameId) {
                  const frame = document.getElementById(frameId);
                  frame.remove();
                }", frameId);

        public static IEnumerable<string> DumpFrames(Frame frame, string indentation = "")
        {
            var description = indentation + Regex.Replace(frame.Url, @":\d{4}", ":<PORT>");
            if (!string.IsNullOrEmpty(frame.Name))
            {
                description += $" ({frame.Name})";
            }
            var result = new List<string>() { description };
            foreach (var child in frame.ChildFrames)
            {
                result.AddRange(DumpFrames(child, "    " + indentation));
            }

            return result;
        }

        internal static Task NavigateFrameAsync(Page page, string frameId, string url)
            => NavigateFrameAsync(page.MainFrame, frameId, url);

        internal static async Task NavigateFrameAsync(Frame frame, string frameId, string url)
        {
            await frame.EvaluateFunctionAsync(@"function navigateFrame(frameId, url) {
              const frame = document.getElementById(frameId);
              frame.src = url;
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }
    }
}
