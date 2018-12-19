using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PuppeteerLaunchTests : PuppeteerBaseTest
    {
        public PuppeteerLaunchTests(ITestOutputHelper output) : base(output) { }

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
                    new NavigationOptions
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
                var exception = await Assert.ThrowsAsync<EvaluationFailedException>(() => neverResolves);
                Assert.IsType<TargetClosedException>(exception.InnerException);
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
            using (var userDataDir = new TempDirectory())
            {
                var options = TestConstants.DefaultBrowserOptions();
                options.UserDataDir = userDataDir.Path;

                var launcher = new Launcher(TestConstants.LoggerFactory);
                using (var browser = await launcher.LaunchAsync(options))
                {
                    Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
                    await browser.CloseAsync();
                    Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
                }
            }
        }

        [Fact]
        public async Task UserDataDirArgument()
        {
            using (var userDataDir = new TempDirectory())
            {
                var launcher = new Launcher(TestConstants.LoggerFactory);
                var options = TestConstants.DefaultBrowserOptions();
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();

                using (var browser = await launcher.LaunchAsync(options))
                {
                    // Open a page to make sure its functional.
                    await browser.NewPageAsync();
                    Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
                    await browser.CloseAsync();
                    Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
                }
            }
        }

        [Fact]
        public async Task UserDataDirOptionShouldRestoreState()
        {
            using (var userDataDir = new TempDirectory())
            {
                var launcher = new Launcher(TestConstants.LoggerFactory);
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
                    Assert.Equal("hello", await page2.EvaluateExpressionAsync<string>("localStorage.hey"));
                }
            }
        }

        [Fact]
        public async Task UserDataDirOptionShouldRestoreCookies()
        {
            using (var userDataDir = new TempDirectory())
            {
                var launcher = new Launcher(TestConstants.LoggerFactory);
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
                    Assert.Equal("doSomethingOnlyOnce=true", await page2.EvaluateExpressionAsync<string>("document.cookie"));
                }
            }
        }

        [Fact]
        public void ShouldReturnTheDefaultChromeArguments()
        {
            Assert.Contains("--no-first-run", Puppeteer.GetDefaultArgs());
            Assert.Contains("--headless", Puppeteer.GetDefaultArgs());
            Assert.DoesNotContain("--headless", Puppeteer.GetDefaultArgs(new LaunchOptions
            {
                Headless = false
            }));
            Assert.Contains("--user-data-dir=\"foo\"", Puppeteer.GetDefaultArgs(new LaunchOptions
            {
                UserDataDir = "foo"
            }));
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

                Assert.True(launcher.Process.HasExited);
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

            Assert.True(await launcher.Process.WaitForExitAsync(TimeSpan.FromSeconds(10)));
            Assert.True(launcher.Process.HasExited);
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
            var process = GetTestAppProcess(
                "PuppeteerSharp.Tests.DumpIO",
                $"{dumpioTextToLog} \"{new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath}\"");

            process.ErrorDataReceived += (sender, e) =>
            {
                success |= e.Data != null && e.Data.Contains(dumpioTextToLog);
            };

            process.Start();
            process.BeginErrorReadLine();
            process.WaitForExit();
            Assert.True(success);
        }

        [Fact]
        public async Task ShouldCloseTheBrowserWhenTheConnectedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            var chromiumProcess = new ChromiumProcess(
                new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath,
                new LaunchOptions { Headless = true },
                TestConstants.LoggerFactory);

            await chromiumProcess.StartAsync().ConfigureAwait(false);

            var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = chromiumProcess.EndPoint
            });

            browser.Disconnected += (sender, e) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(chromiumProcess.Process.Id);

            await browserClosedTaskWrapper.Task;
            Assert.True(browser.IsClosed);
        }

        [Fact]
        public async Task ShouldCloseTheBrowserWhenTheLaunchedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }, TestConstants.LoggerFactory);

            browser.Disconnected += (sender, e) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(browser.ChromiumProcess.Process.Id);

            await browserClosedTaskWrapper.Task;
            Assert.True(browser.IsClosed);
        }

        [Fact]
        public async Task ShouldWorkWithNoDefaultArguments()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreDefaultArgs = true;
            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                Assert.Single(await browser.PagesAsync());
                using (var page = await browser.NewPageAsync())
                {
                    Assert.Equal(121, await page.EvaluateExpressionAsync<int>("11 * 11"));
                }
            }
        }

        [Fact]
        public async Task ShouldFilterOutIgnoredDefaultArguments()
        {
            var defaultArgs = Puppeteer.GetDefaultArgs();
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoredDefaultArgs = new[] { defaultArgs[0], defaultArgs[2] };
            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                var spawnargs = browser.Process.StartInfo.Arguments;
                Assert.DoesNotContain(defaultArgs[0], spawnargs);
                Assert.Contains(defaultArgs[1], spawnargs);
                Assert.DoesNotContain(defaultArgs[2], spawnargs);
            }
        }

        [Fact]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            using (var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory))
            {
                var pages = (await browser.PagesAsync()).Select(page => page.Url);
                Assert.Equal(new[] { TestConstants.AboutBlank }, pages);
            }
        }

        [Fact]
        public async Task ShouldHaveCustomUrlWhenLaunchingBrowser()
        {
            var customUrl = TestConstants.EmptyPage;
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Prepend(customUrl).ToArray();
            using (var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                var pages = await browser.PagesAsync();
                Assert.Single(pages);
                if (pages[0].Url != customUrl)
                {
                    await pages[0].WaitForNavigationAsync();
                }
                Assert.Equal(customUrl, pages[0].Url);
            }
        }

        [Fact]
        public async Task ShouldSetTheDefaultViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = new ViewPortOptions
            {
                Width = 456,
                Height = 789
            };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                Assert.Equal(456, await page.EvaluateExpressionAsync<int>("window.innerWidth"));
                Assert.Equal(789, await page.EvaluateExpressionAsync<int>("window.innerHeight"));
            }
        }

        [Fact]
        public async Task ShouldDisableTheDefaultViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                Assert.Null(page.Viewport);
            }
        }

        [Fact]
        public async Task ShouldTakeFullPageScreenshotsWhenDefaultViewportIsNull()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                Assert.NotEmpty(await page.ScreenshotDataAsync(new ScreenshotOptions { FullPage = true }));
            }
        }

        [Fact]
        public async Task ShouldSupportCustomWebSocket()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var customSocketCreated = false;
            options.WebSocketFactory = (uri, socketOptions, cancellationToken) =>
            {
                customSocketCreated = true;
                return Connection.DefaultWebSocketFactory(uri, socketOptions, cancellationToken);
            };

            using (await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customSocketCreated);
            }
        }

        private Process GetTestAppProcess(string appName, string arguments)
        {
            var process = new Process();

#if NETCOREAPP
            process.StartInfo.WorkingDirectory = GetSubprocessWorkingDir(appName);
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"{appName}.dll {arguments}";
#else
            process.StartInfo.FileName = Path.Combine(GetSubprocessWorkingDir(appName), $"{appName}.exe");
            process.StartInfo.Arguments = arguments;
#endif
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            return process;
        }

        private string GetSubprocessWorkingDir(string dir)
        {
#if DEBUG
            var build = "Debug";
#else
            
            var build = "Release";
#endif
#if NETCOREAPP
            return Path.Combine(
                TestUtils.FindParentDirectory("lib"),
                dir,
                "bin",
                build,
                "netcoreapp2.0");
#else
            return Path.Combine(
                TestUtils.FindParentDirectory("lib"),
                dir,
                "bin",
                build,
                "net471");
#endif
        }

        private void KillProcess(int pid)
        {
            var process = new Process();

            //We need to kill the process tree manually
            //See: https://github.com/dotnet/corefx/issues/26234
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo.FileName = "taskkill";
                process.StartInfo.Arguments = $"-pid {pid} -t -f";
            }
            else
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"kill -s 9 {pid}\"";
            }

            process.Start();
            process.WaitForExit();
        }
    }
}