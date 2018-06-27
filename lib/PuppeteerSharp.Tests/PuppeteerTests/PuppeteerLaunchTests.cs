using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ITestOutputHelper _output;

        public PuppeteerLaunchTests(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShouldSupportIgnoreHTTPSErrorsOption()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
                Assert.Equal(HttpStatusCode.OK, response.Status);
                Assert.NotNull(response.SecurityDetails);
                Assert.Equal("TLS 1.2", response.SecurityDetails.Protocol);
            }
        }

        [Fact]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var responses = new List<Response>();
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            HttpsServer.SetRedirect("/plzredirect", "/empty.html");

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            using (var page = await browser.NewPageAsync())
            {
                page.Response += (sender, e) => responses.Add(e.Response);

                await page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");

                Assert.Equal(2, responses.Count);
                Assert.Equal(HttpStatusCode.Found, responses[0].Status);
                Assert.Equal("TLS 1.2", responses[0].SecurityDetails.Protocol);
            }
        }

        [Fact]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
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

            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
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

        [Fact]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            using (var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory))
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
                return Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
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

            using (var browser = await launcher.LaunchAsync(options))
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
            options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options))
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
            options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync("localStorage.hey = 'hello'");
            }

            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
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
            options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();

            using (var browser = await launcher.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync(
                    "document.cookie = 'doSomethingOnlyOnce=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }

            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
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
            options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();
            options.Headless = false;

            using (var browser = await launcher.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync(
                    "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }

            options.Headless = true;
            using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
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

            using (var browser = await launcher.LaunchAsync(options))
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

            using (var browser = await launcher.LaunchAsync(options))
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
            using (var browser = await launcher.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                {
                    return launcher.LaunchAsync(TestConstants.DefaultBrowserOptions());
                });
                Assert.Equal("Unable to create or connect to another chromium process", exception.Message);
            }
        }

        [Fact]
        public void ShouldDumpBrowserProcessStderr()
        {
            var dumpioTextToLog = "MAGIC_DUMPIO_TEST";
            var success = false;
            var process = new DirectoryInfo(Directory.GetCurrentDirectory()).Name.Contains("netcore") ?
                GetDumpIOCoreProcess(dumpioTextToLog) :
                GetDumpIOFrameworkProcess(dumpioTextToLog);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;

            process.ErrorDataReceived += (sender, e) =>
            {
                success |= e.Data != null && e.Data.Contains(dumpioTextToLog);
            };

            _output.WriteLine(process.StartInfo.WorkingDirectory);
            process.Start();
            process.BeginErrorReadLine();
            process.WaitForExit();
            Assert.True(success);
        }

        private Process GetDumpIOFrameworkProcess(string dumpioTextToLog)
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = GetDumpIOAppDirectory(
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent);
            process.StartInfo.FileName = "PuppeteerSharp.Tests.DumpIO.exe";
            process.StartInfo.Arguments = $"{dumpioTextToLog} " +
                $"\"{new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath}\"";

            return process;
        }

        private Process GetDumpIOCoreProcess(string dumpioTextToLog)
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = GetDumpIOAppDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()));
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"PuppeteerSharp.Tests.DumpIO.dll {dumpioTextToLog} " +
                $"\"{new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath}\"";

            return process;
        }

        private string GetDumpIOAppDirectory(DirectoryInfo baseDir)
        {
            var directoryInfo = baseDir;
            var build = directoryInfo.FullName.Contains("Debug") ? "Debug" : "Release";
            return Path.Combine(
                TestUtils.FindParentDirectory("lib"),
                "PuppeteerSharp.Tests.DumpIO",
                "bin",
                build,
                directoryInfo.Name);
        }
    }
}