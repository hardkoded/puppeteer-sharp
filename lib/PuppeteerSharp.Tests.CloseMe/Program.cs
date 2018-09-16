using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.CloseMe
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = true,
                DumpIO = false,
                ExecutablePath = args[0]
            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                Console.WriteLine(browser.WebSocketEndpoint);
                Console.ReadLine();
            }
        }
    }
}