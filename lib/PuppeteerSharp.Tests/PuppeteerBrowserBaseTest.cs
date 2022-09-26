using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest, IAsyncLifetime
    {
        protected IBrowser Browser { get; set; }
        protected LaunchOptions DefaultOptions { get; set; }

        public PuppeteerBrowserBaseTest(ITestOutputHelper output) : base(output)
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public virtual async Task InitializeAsync()
            => Browser = await Puppeteer.LaunchAsync(
                DefaultOptions ?? TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);

        public virtual async Task DisposeAsync() => await Browser.CloseAsync();
    }
}
