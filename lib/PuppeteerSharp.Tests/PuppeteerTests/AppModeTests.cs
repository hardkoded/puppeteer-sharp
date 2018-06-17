using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AppModeTests : PuppeteerBaseTest
    {
        public AppModeTests(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "WIP")]
        public async Task ShouldWork()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.AppMode = true;

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                Assert.Equal(121, await page.EvaluateExpressionAsync<int>("11 * 11"));
            }
        }
    }
}