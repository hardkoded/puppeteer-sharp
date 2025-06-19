using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest
    {
        protected IBrowser Browser { get; set; }

        protected LaunchOptions DefaultOptions { get; set; }

        [OneTimeSetUp]
        public async Task InitializeAsync()
            => Browser = await Puppeteer.LaunchAsync(
                DefaultOptions ?? TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            if (Browser is not null)
            {
                await Browser.DisposeAsync();
            }
        }
    }
}
