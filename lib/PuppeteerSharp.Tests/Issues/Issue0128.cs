using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0128
    {
        [Fact]
        public async Task LauncherShouldFailGracefully()
        {
            await Assert.ThrowsAsync<ChromiumProcessException>(async () =>
            {
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = new[] { "-remote-debugging-port=-2" };
                await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            });
        }
    }
}