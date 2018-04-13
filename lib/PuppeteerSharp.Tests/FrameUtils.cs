using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class FrameUtils
    {
        public static async Task AttachFrame(PuppeteerSharp.Page page, string frameId, string url)
        {
            await page.EvaluateFunctionAsync(@"(frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }

        public static string DumpFrames(Frame frame, string indentation = "")
        {
            var result = indentation + Regex.Replace(frame.Url, @":\d{4}", ":<PORT>");
            foreach (var child in frame.ChildFrames)
            {
                result += Environment.NewLine + DumpFrames(child, "    " + indentation);
            }

            return result;
        }
    }
}
