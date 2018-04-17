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

            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
                Assert.Equal(response.Status, HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();

            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync("https://www.google.com");
                Assert.Equal(response.Status, HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task ShouldWorkInRealLifeWithOptions()
        {
            var options = TestConstants.DefaultBrowserOptions();

            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(
                    "https://www.google.com",
                    new NavigationOptions()
                    {
                        Timeout = 5000,
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                    });
                Assert.Equal(response.Status, HttpStatusCode.OK);
            }
        }

        [Fact(Skip = "test is not part of v1.0.0")]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var responses = new List<Response>();
                page.ResponseCreated += (sender, e) => responses.Add(e.Response);

                await page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");
                Assert.Equal(2, responses.Count);
                Assert.Equal(HttpStatusCode.Redirect, responses[0].Status);
                var securityDetails = responses[0].SecurityDetails;
                Assert.Equal("TLS 1.2", securityDetails.Protocol);
            }
        }

        [Fact(Skip = "https://github.com/kblok/puppeteer-sharp/issues/76")]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(),
                                                                            TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var neverResolves = page.EvaluateFunctionHandle("() => new Promise(r => {})");
                await browser.CloseAsync();

                await neverResolves;
                var exception = await Assert.ThrowsAsync<Exception>(() => neverResolves);
                Assert.Contains("Protocol error", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldRejectIfExecutablePathIsInvalid()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ExecutablePath = "random-invalid-path";

            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
            {
                return PuppeteerSharp.Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision);
            });

            Assert.Equal("Failed to launch chrome! path to executable does not exist", exception.Message);
            Assert.Equal(options.ExecutablePath, exception.FileName);
        }

        [Fact]
        public async Task UserDataDirOption()
        {
            var launcher = new Launcher();
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir;

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            {
                Assert.True(Directory.GetFiles(userDataDir).Length > 0);
                await browser.CloseAsync();
                Assert.True(Directory.GetFiles(userDataDir).Length > 0);
                await launcher.TryDeleteUserDataDir();
            }
        }

        [Fact]
        public async Task UserDataDirArgument()
        {
            var launcher = new Launcher();
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir={userDataDir}" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            {
                Assert.True(Directory.GetFiles(userDataDir).Length > 0);
                await browser.CloseAsync();
                Assert.True(Directory.GetFiles(userDataDir).Length > 0);
                await launcher.TryDeleteUserDataDir();
            }
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

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(response.Status, HttpStatusCode.OK);

                await browser.CloseAsync();

                Assert.True(launcher.IsChromeClosed);
            }
        }

        [Fact]
        public async Task ChromeShouldBeClosedOnDispose()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher();

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(response.Status, HttpStatusCode.OK);
            }

            Assert.True(launcher.IsChromeClosed);
        }
    }
}
