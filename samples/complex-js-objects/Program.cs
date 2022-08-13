using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp.OffScreen;
using CefSharp.Dom;
using Nito.AsyncEx;
using CefSharp;

namespace Example.ComplexJSObjects
{
    class Program
    {
        [DebuggerDisplay("Title: {Title} Url: {Url}")]
        public class Data
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public override string ToString() => $"Title: {Title} \nURL: {Url}";
        }

        public static int Main(string[] args) => AsyncContext.Run(AsyncMain);

        static async Task<int> AsyncMain()
        {
            Console.WriteLine("Navigating to https://stackoverflow.com/questions/tagged/cefsharp");

            using var chromiumWebBrowser = new ChromiumWebBrowser("https://stackoverflow.com/questions/tagged/cefsharp");

            await chromiumWebBrowser.WaitForInitialLoadAsync();

            var devtolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            Console.WriteLine("Get all urls from page");
            var jsCode = @"() => {
                const selectors = Array.from(document.querySelectorAll('.s-post-summary--content-title .s-link'));
                return selectors.map( t=> {return { title: t.innerHTML, url: t.href}});
            }";
            var results = await devtolsContext.EvaluateFunctionAsync<Data[]>(jsCode);
            foreach (var result in results)
            {
                Console.WriteLine(result.ToString());
            }

            await devtolsContext.DisposeAsync();

            Cef.Shutdown();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return 0;
        }
    }
}
