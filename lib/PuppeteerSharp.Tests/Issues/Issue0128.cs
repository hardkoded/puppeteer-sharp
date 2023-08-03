using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0128
    {
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task LauncherShouldFailGracefully()
        {
            Assert.ThrowsAsync<ProcessException>(async () =>
            {
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = new[] { "--remote-debugging-port=-2" };
                await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            });
        }
    }
}