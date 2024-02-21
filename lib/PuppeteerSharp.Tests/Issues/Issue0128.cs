using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0128
    {
        public void LauncherShouldFailGracefully()
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
