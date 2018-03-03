using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Xunit;

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
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();
        }

        [Fact]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync("https://www.google.com");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();
        }

        [Fact]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();
            
            var responses = new List<Response>();
            page.ResponseCreated += (sender, e) => responses.Add(e.Response);

            await page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");
            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.Redirect, responses[0].Status);
            var securityDetails = responses[0].SecurityDetails;
            Assert.Equal("TLS 1.2", securityDetails.Protocol);

            await page.CloseAsync();
            await browser.CloseAsync();
        }

        [Fact(Skip = "https://github.com/kblok/puppeteer-sharp/issues/76")]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var neverResolves = page.EvaluateHandle("() => new Promise(r => {})");
            await browser.CloseAsync();

            await neverResolves;
            var exception = await Assert.ThrowsAsync<Exception>(() => neverResolves);
            Assert.Contains("Protocol error", exception.Message);
        }

        [Fact]
        public async Task ShouldRejectIfExecutablePathIsInvalid()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ExecutablePath = "random-invalid-path";

            var exception = await Assert.ThrowsAsync<Exception>(() => PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision));
            Assert.Equal("Failed to create connection", exception.Message);
            Assert.IsType<Win32Exception>(exception.InnerException);
            Assert.Equal("The system cannot find the file specified", exception.InnerException.Message);
        }
    }
}
