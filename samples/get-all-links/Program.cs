using System;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp.DevTools.Dom;
using Nito.AsyncEx;
using CefSharp;

namespace Example.GetAllLinks
{
    public class Program
    {
        public static int Main(string[] args) => AsyncContext.Run(AsyncMain);

        public static async Task<int> AsyncMain()
        {
            using var chromiumWebBrowser = new ChromiumWebBrowser("https://github.com");

            await chromiumWebBrowser.WaitForInitialLoadAsync();

            var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            Console.WriteLine("Navigating to google.com");

            await devtoolsContext.GoToAsync("http://www.google.com");
            var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            var urls = await devtoolsContext.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
            foreach (string url in urls)
            {
                Console.WriteLine($"Url: {url}");
            }

            await devtoolsContext.DisposeAsync();

            Cef.Shutdown();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return 0;
        }
    }
}
