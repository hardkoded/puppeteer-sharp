using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public Puppeteer()
        {
        }

        public static async Task<Browser> LaunchAsync(Dictionary<string, object> options, int chromiumRevision)
        {
            return await Launcher.LaunchAsync(options, chromiumRevision);
        }
    }
}
