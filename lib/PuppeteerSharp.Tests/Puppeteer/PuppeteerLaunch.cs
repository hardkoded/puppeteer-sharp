using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Xunit;
using PuppeteerSharp;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.Puppeteer
{
    public class PuppeteerLaunch
    {
        private const int HttpsPort = 8908;
        private const string HttpsPrefix = "https://localhost:8908";
        private const string ChromiumRevision = "526987";

        private Dictionary<string, object> _defaultBrowserOptions = new Dictionary<string, object>()
        {
            { "slowMo", Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO") ?? "0") },
            { "headless", Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true") },
            { "args", new[] { "--no-sandbox" }},
            { "timeout", 0}
        };

        public PuppeteerLaunch()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(ChromiumRevision).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task should_support_ignoreHTTPSErrors_option()
        {
            var options = _defaultBrowserOptions.Clone();
            options.Add("ignoreHTTPSErrors", true);

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(HttpsPrefix + "/empty.html");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();
        }
    }
}
