using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.DumpIO
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Debug.Assert(args != null, nameof(args) + " != null");
            var options = new LaunchOptions
            {
                Headless = true,
                DumpIO = true,
                ExecutablePath = args[0]
            };

            if (args[1] == "firefox")
            {
                options.Browser = SupportedBrowser.Firefox;
            }

            var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
