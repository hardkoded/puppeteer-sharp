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
    public class PuppeteerLaunchTests : PuppeteerBaseTest
    {
        public PuppeteerLaunchTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync("https://www.google.com");
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldWorkInRealLifeWithOptions()
        {
            var options = TestConstants.DefaultBrowserOptions();

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(
                    "https://www.google.com",
                    new NavigationOptions()
                    {
                        Timeout = 10000,
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                    });
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact(Skip = "test is not part of v1.0.0")]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var responses = new List<Response>();
                page.Response += (sender, e) => responses.Add(e.Response);

                await page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");
                Assert.Equal(2, responses.Count);
                Assert.Equal(HttpStatusCode.Redirect, responses[0].Status);
                var securityDetails = responses[0].SecurityDetails;
                Assert.Equal("TLS 1.2", securityDetails.Protocol);
            }
        }

        [Fact]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            using (var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var neverResolves = page.EvaluateFunctionHandleAsync("() => new Promise(r => {})");
                await browser.CloseAsync();
                var exception = await Assert.ThrowsAsync<TargetClosedException>(() => neverResolves);
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
                return Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory);
            });

            Assert.Equal("Failed to launch chrome! path to executable does not exist", exception.Message);
            Assert.Equal(options.ExecutablePath, exception.FileName);
        }

        [Fact]
        public async Task UserDataDirOption()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
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
            var launcher = new Launcher(TestConstants.LoggerFactory);
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
        public async Task UserDataDirOptionShouldRestoreState()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir={userDataDir}" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync("localStorage.hey = 'hello'");
            }

            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            {
                var page2 = await browser2.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("hello", await page2.EvaluateExpressionAsync("localStorage.hey"));
            }

            await launcher.TryDeleteUserDataDir();
        }

        [Fact]
        public async Task UserDataDirOptionShouldRestoreCookies()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir={userDataDir}" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync(
                    "document.cookie = 'doSomethingOnlyOnce=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }

            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            {
                var page2 = await browser2.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("doSomethingOnlyOnce=true", await page2.EvaluateExpressionAsync("document.cookie"));
            }

            await launcher.TryDeleteUserDataDir();
        }

        [Fact]
        public async Task HeadlessShouldBeAbleToReadCookiesWrittenByHeadful()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var userDataDir = Launcher.GetTemporaryDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir={userDataDir}" }).ToArray();
            options.Headless = false;

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync(
                    "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }

            options.Headless = true;
            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.ChromiumRevision, TestConstants.LoggerFactory))
            {
                var page2 = await browser2.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal("foo=true", await page2.EvaluateExpressionAsync("document.cookie"));
            }

            await launcher.TryDeleteUserDataDir();
        }

        [Fact]
        public void ShouldReturnTheDefaultChromeArguments()
        {
            var args = Puppeteer.DefaultArgs;
            Assert.Contains("--no-first-run", args);
        }

        [Fact]
        public async Task ChromeShouldBeClosed()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.OK, response.Status);

                await browser.CloseAsync();

                Assert.True(launcher.IsChromeClosed);
            }
        }

        [Fact]
        public async Task ChromeShouldBeClosedOnDispose()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            using (var browser = await launcher.LaunchAsync(options, TestConstants.ChromiumRevision))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }

            Assert.True(launcher.IsChromeClosed);
        }

        [Fact]
        public async Task ShouldNotOpenTwoChromesUsingTheSameLauncher()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            using (var browser = await launcher.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.ChromiumRevision))
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                {
                    return launcher.LaunchAsync(
                        TestConstants.DefaultBrowserOptions(),
                        TestConstants.ChromiumRevision);
                });
                Assert.Equal("Unable to create or connect to another chromium process", exception.Message);
            }
        }
    }
}
