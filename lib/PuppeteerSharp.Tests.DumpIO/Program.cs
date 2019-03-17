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
                ExecutablePath = args[0]
            };

            var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}