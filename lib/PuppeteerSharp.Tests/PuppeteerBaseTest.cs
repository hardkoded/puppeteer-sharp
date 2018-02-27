using System;
using System.IO;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBaseTest : IDisposable
    {
        internal string BaseDirectory { get; set; }
        internal Browser Browser { get; set; }

        public PuppeteerBaseTest()
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Browser = PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(),
                                                           TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Browser.CloseAsync().GetAwaiter().GetResult();
        }
    }
}
