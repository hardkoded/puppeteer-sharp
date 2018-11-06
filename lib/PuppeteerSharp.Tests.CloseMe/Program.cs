using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.CloseMe
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = false,
                DumpIO = false,
                Timeout = 0,
                ExecutablePath = @"D:\Git\better\Console\bin\Debug\.local-chromium\Win64-594312\chrome-win\chrome.exe"
            };

            //var fetcher = Puppeteer.CreateBrowserFetcher(new BrowserFetcherOptions());
            //   await fetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                page.DefaultWaitForTimeout = 0;
                page.DefaultNavigationTimeout = 0;
                await page.GoToAsync("https://www.anbodianjing.com");
                await Task.Delay(5000);
                //var applicationCache = await page.MainFrame.GetApplicationCache();
                Console.WriteLine(browser.WebSocketEndpoint);
                Console.ReadLine();
            }
        }
    }
}