using System;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest, IDisposable
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

            InitializeAsync().GetAwaiter().GetResult();
        }

        protected virtual async Task InitializeAsync()
        {
            Browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision, TestConstants.LoggerFactory);
        }

        protected virtual async Task DisposeAsync() => await Browser.CloseAsync();
        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
    }
}