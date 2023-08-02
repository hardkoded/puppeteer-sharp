using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0128
    {
        [SkipBrowserFact(skipFirefox: true)]
        public async Task LauncherShouldFailGracefully()
        {
            await Assert.ThrowsAsync<ProcessException>(async () =>
            {
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = new[] { "--remote-debugging-port=-2" };
                await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            });
        }
    }
}