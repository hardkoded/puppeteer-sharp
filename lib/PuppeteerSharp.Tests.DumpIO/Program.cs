using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.DumpIO
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var supportedBrowser = args[1] == "chrome" ? SupportedBrowser.Chrome : SupportedBrowser.Firefox;
            var options = new LaunchOptions
            {
                Browser = supportedBrowser,
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
