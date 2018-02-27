using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public static async Task<Browser> LaunchAsync(LaunchOptions options, int chromiumRevision)
        {
            return await new Launcher().LaunchAsync(options, chromiumRevision);
        }
    }
}
