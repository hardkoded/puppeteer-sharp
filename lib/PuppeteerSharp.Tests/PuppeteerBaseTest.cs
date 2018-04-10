using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PuppeteerBaseTest : IAsyncLifetime
    {
        protected string BaseDirectory { get; set; }
        protected Browser Browser { get; set; }

        public virtual async Task InitializeAsync()
        {
            Browser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision);
        }

        public virtual async Task DisposeAsync() => await Browser.CloseAsync();

        public PuppeteerBaseTest()
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
    }
}