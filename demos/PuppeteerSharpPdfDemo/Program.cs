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

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();

            Console.WriteLine("Evaluating page content");
            var bodyContent = await page.EvaluateFunctionAsync<string>(
                @"() => {
                    document.body.innerHTML = '<h1>Hello from PuppeteerSharp Firefox!</h1>';
                    return document.body.innerText;
                }");
            Console.WriteLine($"Body content: {bodyContent}");

#if NET8_0_OR_GREATER
            // AOT Test - serialize TestClass as argument, return a string to avoid
            // reflection-based deserialization which is not yet AOT-compatible in BiDi
            var result = await page.EvaluateFunctionAsync<string>("test => test.Name", new TestClass { Name = "Dario"});
            Console.WriteLine($"Name evaluated to {result}");
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
