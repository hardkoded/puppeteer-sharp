using System;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Example.Searching
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions { Headless = true };
            Console.WriteLine("Downloading chromium");

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Console.WriteLine("Navigating to developers.google.com");

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://developers.google.com/web/");
                // Type into search box.
                await page.TypeAsync("#searchbox input", "Headless Chrome");

                // Wait for suggest overlay to appear and click "show all results".
                var allResultsSelector = ".devsite-suggest-all-results";
                await page.WaitForSelectorAsync(allResultsSelector);
                await page.ClickAsync(allResultsSelector);

                // Wait for the results page to load and display the results.
                var resultsSelector = ".gsc-results .gsc-thumbnail-inside a.gs-title";
                await page.WaitForSelectorAsync(resultsSelector);
                var links = await page.EvaluateFunctionAsync(@"(resultsSelector) => {
    const anchors = Array.from(document.querySelectorAll(resultsSelector));
    return anchors.map(anchor => {
      const title = anchor.textContent.split('|')[0].trim();
      return `${title} - ${anchor.href}`;
    });
}", resultsSelector);

                foreach (var link in links)
                {
                    Console.WriteLine(link); 
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
