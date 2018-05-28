using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerSharpPdfDemo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = true
            };

            Console.WriteLine("Downloading chromium");
            await Downloader.CreateDefault().DownloadRevisionAsync(Downloader.DefaultRevision);

            Console.WriteLine("Navigating google");
            using (var browser = await Puppeteer.LaunchAsync(options, Downloader.DefaultRevision))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("http://www.google.com");

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
}
