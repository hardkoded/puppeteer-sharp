using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest, IAsyncLifetime
    {
        protected Browser Browser { get; set; }

        public PuppeteerBrowserBaseTest(ITestOutputHelper output) : base(output)
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        Task IAsyncLifetime.InitializeAsync() => InitializeAsync();

        protected virtual async Task InitializeAsync() => 
            Browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);

        Task IAsyncLifetime.DisposeAsync() => DisposeAsync();

        protected virtual async Task DisposeAsync() => await Browser.CloseAsync();
    }
}