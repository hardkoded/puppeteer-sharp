using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Example.GetAllLinks
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions { Headless = true };
            Console.WriteLine("Downloading chromium");
            await new BrowserFetcher().DownloadAsync();
            Console.WriteLine("Navigating to google.com");

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("http://www.google.com");
                var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                foreach (string url in urls)
                {
                    Console.WriteLine($"Url: {url}");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
