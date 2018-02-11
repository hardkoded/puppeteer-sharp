using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Xunit;
using PuppeteerSharp;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.Puppeteer
{
    public class PuppeteerLaunchTests
    {
        public PuppeteerLaunchTests()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            var options = TestConstants.DefaultBrowserOptions.Clone();
            options.Add("ignoreHTTPSErrors", true);

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();
        }
    }
}
