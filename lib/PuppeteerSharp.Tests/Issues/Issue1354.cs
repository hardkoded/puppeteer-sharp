using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue1354 : PuppeteerPageBaseTest
    {
        public Issue1354(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(Timeout = 30000)]
        public async Task ShouldAllowSyncClose()
        {
            var options = TestConstants.DefaultBrowserOptions();
            if (!options.EnqueueAsyncMessages)
            {
                // This test won't pass unless this option is set to true.
                return;
            }

            using (var browser = await Puppeteer.LaunchAsync(options).ConfigureAwait(false))
            {
                // In issue #1354, this line hangs forever
                browser.CloseAsync().Wait();
            }
        }

        [Fact(Timeout = 30000)]
        public async Task ShouldAllowSyncPageMethod()
        {
            var options = TestConstants.DefaultBrowserOptions();
            if (!options.EnqueueAsyncMessages)
            {
                return;
            }

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                // Async low-level use
                await using var page = await browser.NewPageAsync().ConfigureAwait(false);
                await page.GoToAsync("http://ipecho.net/plain", WaitUntilNavigation.DOMContentLoaded).ConfigureAwait(false);
                await page.SetContentAsync("<html><body>REPLACED</body></html>").ConfigureAwait(false);

                // Deep inside an existing mostly sync app...
                var content = page.GetContentAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.Contains("REPLACE", content);
            }
        }
    }
}
