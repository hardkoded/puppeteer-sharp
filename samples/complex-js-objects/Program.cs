using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace complex_js_objects
{
    class Program
    {
        [DebuggerDisplay("Content: {Content} Url: {Url}")]
        public class Data
        {
            public string Content { get; set; }
            public string Url { get; set; }
        }

        static async Task Main(string[] args)
        {
            var options = new LaunchOptions { Headless = true };
            Console.WriteLine("Downloading chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Console.WriteLine("Navigating to Hacker News");
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://news.ycombinator.com/");
                Console.WriteLine("Get all urls from page");
                var jsCode = @"
const selectors = Array.from(document.querySelectorAll('a[class=""storylink""]')); 
selectors.map( t=> {return {content: t.innerHTML, url: t.href}});
";
                var handles = await page.EvaluateExpressionHandleAsync(jsCode);
                var results = await handles.JsonValueAsync<Data[]>();
                //foreach (string url in urls)
                //{
                //    Console.WriteLine($"Url: {url}");
                //}
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
