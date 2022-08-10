using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp.DevTools.Dom;
using CefSharp;
using Nito.AsyncEx;

namespace Example.Searching
{
    class Program
    {
        public static int Main(string[] args) => AsyncContext.Run(AsyncMain);

        public static async Task<int> AsyncMain()
        {
            using var chromiumWebBrowser = new ChromiumWebBrowser("https://github.com");

            await chromiumWebBrowser.WaitForInitialLoadAsync();

            var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            Console.WriteLine("Navigating to developers.google.com");

            await devtoolsContext.GoToAsync("https://developers.google.com/web/");
            // Type into search box.
            await devtoolsContext.TypeAsync(@"input[name='q']", "Headless Chrome");

            // Wait for suggest overlay to appear and click "show all results".
            var allResultsSelector = ".devsite-suggest-all-results";
            await devtoolsContext.WaitForSelectorAsync(allResultsSelector);
            await devtoolsContext.ClickAsync(allResultsSelector);

            // Wait for the results page to load and display the results.
            var resultsSelector = ".gsc-results .gsc-thumbnail-inside a.gs-title";
            await devtoolsContext.WaitForSelectorAsync(resultsSelector);
            var links = await devtoolsContext.EvaluateFunctionAsync(@"(resultsSelector) => {
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

            await devtoolsContext.DisposeAsync();

            Cef.Shutdown();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return 0;
        }
    }
}
