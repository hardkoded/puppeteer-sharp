using System.Threading.Tasks;
using System.Web.Hosting;
using PuppeteerSharp;
using PuppeteerSharp.AspNetFramework;

namespace PupppeterSharpAspNetFrameworkSample.Services
{
    public class BrowserClient
    {
        private static readonly string HostPath = HostingEnvironment.MapPath("~/App_Data/");
        
        public static async Task<string> GetTextAsync(string url)
        {
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions()
            {
                Path = HostPath
            });
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision).ConfigureAwait(false);

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                Headless = true,
                Transport = new AspNetWebSocketTransport(),
                ExecutablePath = browserFetcher.GetExecutablePath(BrowserFetcher.DefaultRevision)
            }).ConfigureAwait(false))
            using(var page = await browser.NewPageAsync().ConfigureAwait(false))
            {
                var response = await page.GoToAsync(url).ConfigureAwait(false);
                return await response.TextAsync().ConfigureAwait(false);
            }
        }
    }
}