using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerSharpPdfDemo
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
#if NET8_0_OR_GREATER
            Puppeteer.ExtraJsonSerializerContext = DemoJsonSerializationContext.Default;
#endif

            var options = new LaunchOptions 
            { 
                Headless = true,
                Browser = SupportedBrowser.Firefox
            };

            Console.WriteLine("Downloading Firefox");

            var browserFetcher = new BrowserFetcher(SupportedBrowser.Firefox);
            await browserFetcher.DownloadAsync();

            Console.WriteLine("Navigating to mabl.com");
            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();

            await page.GoToAsync("https://www.mabl.com", new NavigationOptions
            {
                Timeout = 120_000,
                WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
            });

            Console.WriteLine("Checking page content");
            var bodyContent = await page.EvaluateFunctionAsync<string>("() => document.body.innerText");
            Console.WriteLine($"Body has {bodyContent.Length} characters");

#if NET8_0_OR_GREATER
            // AOT Test
            var result = await page.EvaluateFunctionAsync<TestClass>("test => test", new TestClass { Name = "Dario"});
            Console.WriteLine($"Name evaluated to {result.Name}");
#endif
            if (!args.Any(arg => arg == "auto-exit"))
            {
                Console.ReadLine();
            }
        }
    }

#if NET8_0_OR_GREATER
    public class TestClass
    {
        public string Name { get; set; }
    }

    [JsonSerializable(typeof(TestClass))]
    public partial class DemoJsonSerializationContext : JsonSerializerContext
    {}
#endif
}
