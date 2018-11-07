using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Example.ComplexJSObjects
{
    class Program
    {
        [DebuggerDisplay("Content: {Content} Url: {Url}")]
        public class Data
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public override string ToString() => $"Title: {Title} \nURL: {Url}";
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
                var jsCode = @"() => {
const selectors = Array.from(document.querySelectorAll('a[class=""storylink""]')); 
return selectors.map( t=> {return { title: t.innerHTML, url: t.href}});
}";
                var results = await page.EvaluateFunctionAsync<Data[]>(jsCode);
                foreach (var result in results)
                {
                    Console.WriteLine(result.ToString());
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
