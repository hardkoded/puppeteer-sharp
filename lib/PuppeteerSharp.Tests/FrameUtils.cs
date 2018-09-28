using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class FrameUtils
    {
        public static async Task AttachFrameAsync(Page page, string frameId, string url)
        {
            await page.EvaluateFunctionAsync(@"(frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }

        public static async Task DetachFrameAsync(Page page, string frameId)
        {
            await page.EvaluateFunctionAsync(@"function detachFrame(frameId) {
              const frame = document.getElementById(frameId);
              frame.remove();
            }", frameId);
        }

        public static string DumpFrames(Frame frame, string indentation = "")
        {
            var result = indentation + Regex.Replace(frame.Url, @":\d{4}", ":<PORT>");
            foreach (var child in frame.ChildFrames)
            {
                result += "\n" + DumpFrames(child, "    " + indentation);
            }

            return result;
        }

        internal static async Task NavigateFrameAsync(Page page, string frameId, string url)
        {
            await page.EvaluateFunctionAsync(@"function navigateFrame(frameId, url) {
              const frame = document.getElementById(frameId);
              frame.src = url;
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }
    }
}
