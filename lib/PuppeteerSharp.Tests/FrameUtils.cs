using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class FrameUtils
    {
        public static Task<IFrame> AttachFrameAsync(IPage page, string frameId, string url)
            => AttachFrameAsync(page.MainFrame, frameId, url);

        public static async Task<IFrame> AttachFrameAsync(IFrame frame, string frameId, string url)
        {
            var handle = (IElementHandle)await frame.EvaluateFunctionHandleAsync(@" async (frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              await new Promise(x => frame.onload = x);
              return frame
            }", frameId, url);
            return await handle.ContentFrameAsync();
        }

        public static Task DetachFrameAsync(IPage page, string frameId)
            => DetachFrameAsync(page.MainFrame, frameId);

        public static Task DetachFrameAsync(IFrame frame, string frameId)
            => frame.EvaluateFunctionAsync(@"function detachFrame(frameId) {
                  const frame = document.getElementById(frameId);
                  frame.remove();
                }", frameId);

        public static async Task<IEnumerable<string>> DumpFramesAsync(IFrame frame, string indentation = "")
        {
            var description = indentation + Regex.Replace(frame.Url, @":\d{4}", ":<PORT>");
            await using var frameElement = await frame.FrameElementAsync();
            if (frameElement != null)
            {
                var nameOrId = await frameElement.EvaluateFunctionAsync<string>(@"frame => {
                    return frame.name || frame.id;
                }");
                description += $" ({nameOrId})";
            }
            var result = new List<string>() { description };
            foreach (var child in frame.ChildFrames)
            {
                result.AddRange(await DumpFramesAsync(child, "    " + indentation));
            }

            return result;
        }

        internal static Task NavigateFrameAsync(IPage page, string frameId, string url)
            => NavigateFrameAsync(page.MainFrame, frameId, url);

        internal static async Task NavigateFrameAsync(IFrame frame, string frameId, string url)
        {
            await frame.EvaluateFunctionAsync(@"function navigateFrame(frameId, url) {
              const frame = document.getElementById(frameId);
              frame.src = url;
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }
    }
}
