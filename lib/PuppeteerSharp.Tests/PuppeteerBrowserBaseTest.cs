using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest
    {
        protected IBrowser Browser { get; set; }

        protected LaunchOptions DefaultOptions { get; set; }

        [SetUp]
        public async Task InitializeAsync()
            => Browser = await Puppeteer.LaunchAsync(
                DefaultOptions ?? TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);

        [TearDown]
        public async Task TearDownAsync()
        {
            if (Browser is not null)
            {
                await Browser.DisposeAsync();
            }
        }
    }
}
