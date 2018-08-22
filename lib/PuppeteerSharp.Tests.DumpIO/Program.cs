using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.DumpIO
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = true,
                DumpIO = true,
                ExecutablePath = args[1]
            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.EvaluateFunctionAsync("_dumpioTextToLog => console.log(_dumpioTextToLog)", args[0]);
            }
        }
    }
}