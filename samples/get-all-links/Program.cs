using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp.Puppeteer;

namespace Example.GetAllLinks
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var chromiumWebBrowser = new ChromiumWebBrowser("https://github.com");

            await chromiumWebBrowser.WaitForInitialLoadAsync();

            await using var page = await chromiumWebBrowser.GetPuppeteerPageAsync();

            Console.WriteLine("Navigating to google.com");

            await page.GoToAsync("http://www.google.com");
            var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
            foreach (string url in urls)
            {
                Console.WriteLine($"Url: {url}");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
