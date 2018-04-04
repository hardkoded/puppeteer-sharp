using System;
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
    }
}
