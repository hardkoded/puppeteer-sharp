using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PuppeteerLaunchTests : PuppeteerBaseTest
    {
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

        [Fact(Skip = "test is not part of v1.0.0")]
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

            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() => PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision));
            Assert.Equal("Failed to launch chrome! path to executable does not exist", exception.Message);
            Assert.Equal(options.ExecutablePath, exception.FileName);
        }

        [Fact]
        public async Task UserDataDirOption()
        {
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir;

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            Assert.True(Directory.GetFiles(userDataDir).Length > 0);
            await browser.CloseAsync();
            Assert.True(Directory.GetFiles(userDataDir).Length > 0);
            await Launcher.TryDeleteDirectory(userDataDir);
        }

        [Fact]
        public async Task UserDataDirArgument()
        {
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir={userDataDir}" }).ToArray();

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            Assert.True(Directory.GetFiles(userDataDir).Length > 0);
            await browser.CloseAsync();
            Assert.True(Directory.GetFiles(userDataDir).Length > 0);
            await Launcher.TryDeleteDirectory(userDataDir);
        }

        [Fact]
        public void ShouldReturnTheDefaultChromeArguments()
        {
            var args = PuppeteerSharp.Puppeteer.DefaultArgs;
            Assert.Contains("--no-first-run", args);
        }

        [Fact]
        public async Task ChromeShouldBeClosed()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher();
            var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision);
            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync("https://www.google.com");
            Assert.Equal(response.Status.ToString(), "OK");

            await browser.CloseAsync();

            Assert.True(launcher.IsChromeClosed);
        }
    }
}
