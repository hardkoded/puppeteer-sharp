using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp.Puppeteer;

namespace Example.Searching
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var chromiumWebBrowser = new ChromiumWebBrowser("https://github.com");

            await chromiumWebBrowser.WaitForInitialLoadAsync();

            await using var page = await chromiumWebBrowser.GetPuppeteerPageAsync();

            Console.WriteLine("Navigating to developers.google.com");

            await page.GoToAsync("https://developers.google.com/web/");
            // Type into search box.
            await page.TypeAsync(@"input[name='q']", "Headless Chrome");

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
            Console.ReadKey();
        }
    }
}
