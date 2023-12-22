using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerSharpPdfDemo
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions { Headless = true };

            Console.WriteLine("Downloading chromium");

            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            Console.WriteLine("Navigating google");
            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();

            await page.GoToAsync("https://www.google.com");

            Console.WriteLine("Generating PDF");
            await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "google.pdf"));

            Console.WriteLine("Export completed");

            if (!args.Any(arg => arg == "auto-exit"))
            {
                Console.ReadLine();
            }
        }
    }
}
