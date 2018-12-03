using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using PuppeteerSharp;

namespace Example.ReuseDownloadedChrome
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("This example downloads the correct version of Chromium to a custom location");

            var currentDirectory = Directory.GetCurrentDirectory();
            var downloadPath = Path.Combine(currentDirectory, "..", "..", "CustomChromium");
            Console.WriteLine($"Attemping to set up puppeteer to use Chromium found under directory {downloadPath} ");

            if (!Directory.Exists(downloadPath))
            {
                Console.WriteLine("Custom directory not found. Creating directory");
                Directory.CreateDirectory(downloadPath);
            }

            var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
            var browserFetcher = new BrowserFetcher(browserFetcherOptions);
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

            Console.WriteLine("Create full path to chromium executable");

            var executablePath = CreateExecutablePath(downloadPath);

            if (string.IsNullOrEmpty(executablePath))
            {
                Console.WriteLine("Custom Chromium location is empty. Unable to start Chromium. Exiting.\n Press any key to continue");
                Console.ReadLine();
                return;
            } 

            Console.WriteLine($"Attemping to start Chromium using executable path: {executablePath}");

            var options = new LaunchOptions { Headless = true, ExecutablePath = executablePath };

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
            return;
        }

        public static string CreateExecutablePath(string basePath)
        {
            var firstVersionDirectory = Directory.GetDirectories(basePath).FirstOrDefault();

            if (string.IsNullOrEmpty(firstVersionDirectory))
            {
                return string.Empty;
            }

            var firstPlatformDirectory = Directory.GetDirectories(firstVersionDirectory).FirstOrDefault();
            if (string.IsNullOrEmpty(firstPlatformDirectory))
            {
                return string.Empty;
            }

            return Path.Combine(firstPlatformDirectory, "chrome.exe");
        }
    }
}
