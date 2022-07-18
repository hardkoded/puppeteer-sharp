using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests
{
    public static class FrameUtils
    {
        public static async Task<Frame> AttachFrameAsync(DevToolsContext devToolsContext, string frameId, string url)
        {
            var handle = (ElementHandle)await devToolsContext.EvaluateFunctionHandleAsync(@" async (frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              await new Promise(x => frame.onload = x);
              return frame
            }", frameId, url);
            return await handle.ContentFrameAsync();
        }

        public static async Task DetachFrameAsync(DevToolsContext devToolsContext, string frameId)
        {
            await devToolsContext.EvaluateFunctionAsync(@"function detachFrame(frameId) {
              const frame = document.getElementById(frameId);
              frame.remove();
            }", frameId);
        }

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

        internal static async Task NavigateFrameAsync(DevToolsContext devToolsContext, string frameId, string url)
        {
            await devToolsContext.EvaluateFunctionAsync(@"function navigateFrame(frameId, url) {
              const frame = document.getElementById(frameId);
              frame.src = url;
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }
    }
}
