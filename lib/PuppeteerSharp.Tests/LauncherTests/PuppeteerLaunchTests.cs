using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class PuppeteerLaunchTests : PuppeteerBaseTest
    {
        [Test, Retry(2)]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            var response = await page.GoToAsync("https://www.github.com");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should reject all promises when browser is closed")]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            await using var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            var neverResolves = page.EvaluateFunctionHandleAsync("() => new Promise(r => {})");
            await browser.CloseAsync();
            var exception = Assert.ThrowsAsync<TargetClosedException>(() => neverResolves);
            StringAssert.Contains("Protocol error", exception!.Message);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should reject if executable path is invalid")]
        public void ShouldRejectIfExecutablePathIsInvalid()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ExecutablePath = "random-invalid-path";

            Assert.ThrowsAsync<Win32Exception>(() => Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory));
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option")]
        public async Task UserDataDirOption()
        {
            using var userDataDir = new TempDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;

            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);
            Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
            await browser.CloseAsync();
            Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir argument")]
        public async Task UserDataDirArgument()
        {
            using var userDataDir = new TempDirectory();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = TestConstants.DefaultBrowserOptions();

            if (TestConstants.IsChrome)
            {
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();
            }
            else
            {
                options.Args = options.Args.Concat(new string[] { "-profile", userDataDir.ToString() }).ToArray();
            }

            await using var browser = await launcher.LaunchAsync(options);
            // Open a page to make sure its functional.
            await browser.NewPageAsync();
            Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
            await browser.CloseAsync();
            Assert.True(Directory.GetFiles(userDataDir.Path).Length > 0);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option should restore state")]
        public async Task UserDataDirOptionShouldRestoreState()
        {
            using var userDataDir = new TempDirectory();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = TestConstants.DefaultBrowserOptions();

            if (TestConstants.IsChrome)
            {
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();
            }
            else
            {
                options.Args = options.Args.Concat(new string[] { "-profile", userDataDir.ToString() }).ToArray();
            }

            await using (var browser = await launcher.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync("localStorage.hey = 'hello'");
            }

            await using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                var page2 = await browser2.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual("hello", await page2.EvaluateExpressionAsync<string>("localStorage.hey"));
            }
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option should restore cookies")]
        [Ignore("This mysteriously fails on Windows.")]
        public async Task UserDataDirOptionShouldRestoreCookies()
        {
            using var userDataDir = new TempDirectory();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDir}\"" }).ToArray();

            await using (var browser = await launcher.LaunchAsync(options))
            {
                var page = await browser.NewPageAsync();
                await page.GoToAsync(TestConstants.EmptyPage);
                await page.EvaluateExpressionAsync(
                    "document.cookie = 'doSomethingOnlyOnce=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }
            await TestUtils.WaitForCookieInChromiumFileAsync(userDataDir.Path, "doSomethingOnlyOnce");

            await using (var browser2 = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                var page2 = await browser2.NewPageAsync();
                await page2.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual("doSomethingOnlyOnce=true", await page2.EvaluateExpressionAsync<string>("document.cookie"));
            }
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should return the default arguments")]
        public void ShouldReturnTheDefaultArguments()
        {
            Assert.Contains("--headless=new", Puppeteer.GetDefaultArgs(new LaunchOptions() { Headless = true }));
            Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions
            {
                Headless = false
            }), Does.Not.Contain("--headless"));

            if (TestConstants.IsChrome)
            {
                Assert.Contains("--no-first-run", Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()));
                Assert.Contains("--user-data-dir=\"foo\"", Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo"
                }));
            }
            else
            {
                Assert.Contains("--no-remote", Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.Contains("--foreground", Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()));
                }
                else
                {
                    Assert.That(Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()), Does.Not.Contain("--foreground"));
                }
                Assert.Contains("--profile", Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo",
                    Browser = TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
                }));
                Assert.Contains("\"foo\"", Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo",
                    Browser = TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
                }));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task ChromeShouldBeClosed(bool useDisposeAsync)
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            await using var browser = await launcher.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            if (useDisposeAsync)
            {
                // emulates what would happen in a C#8 await using block
                await browser.DisposeAsync();
            }
            else
            {
                await browser.CloseAsync();
            }

            Assert.True(launcher.Process.HasExited);
        }

        public async Task ChromeShouldBeClosedOnDispose()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            await using (var browser = await launcher.LaunchAsync(options))
            await using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual(HttpStatusCode.OK, response.Status);
            }

            Assert.True(await launcher.Process.WaitForExitAsync(TimeSpan.FromSeconds(10)));
            Assert.True(launcher.Process.HasExited);
        }

        public async Task ShouldNotOpenTwoChromesUsingTheSameLauncher()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using (await launcher.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var exception = Assert.ThrowsAsync<InvalidOperationException>(
                    () => launcher.LaunchAsync(TestConstants.DefaultBrowserOptions()));
                Assert.AreEqual("Unable to create or connect to another process", exception!.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should work with no default arguments")]
        public async Task ShouldWorkWithNoDefaultArguments()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreDefaultArgs = true;
            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            Assert.AreEqual(121, await page.EvaluateExpressionAsync<int>("11 * 11"));
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should filter out ignored default arguments in Chrome")]
        public async Task ShouldFilterOutIgnoredDefaultArguments()
        {
            var defaultArgs = Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions());
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoredDefaultArgs = [defaultArgs[0], defaultArgs[2]];
            await using var browser = await Puppeteer.LaunchAsync(options);
            var spawnArgs = browser.Process.StartInfo.Arguments;
            StringAssert.DoesNotContain(defaultArgs[0], spawnArgs);
            StringAssert.Contains(defaultArgs[1], spawnArgs);
            StringAssert.DoesNotContain(defaultArgs[2], spawnArgs);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should have default URL when launching browser")]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
            var pages = (await browser.PagesAsync()).Select(page => page.Url);
            Assert.AreEqual(new[] { TestConstants.AboutBlank }, pages);
        }


        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should close browser with beforeunload page")]
        public async Task ShouldCloseBrowserWithBeforeunloadPage()
        {
            var headfulOptions = TestConstants.DefaultBrowserOptions();
            headfulOptions.Headless = false;
            await using var browser = await Puppeteer.LaunchAsync(headfulOptions);
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(TestConstants.ServerUrl + "/beforeunload.html");
                // We have to interact with a page so that 'beforeunload' handlers fire.
                await page.ClickAsync("body");
            }
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should have custom URL when launching browser")]
        public async Task ShouldHaveCustomUrlWhenLaunchingBrowser()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Prepend(TestConstants.EmptyPage).ToArray();
            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var pages = await browser.PagesAsync();
            Assert.That(pages, Has.Exactly(1).Items);
            if (pages[0].Url != TestConstants.EmptyPage)
            {
                await pages[0].WaitForNavigationAsync();
            }
            Assert.AreEqual(TestConstants.EmptyPage, pages[0].Url);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should set the default viewport")]
        public async Task ShouldSetTheDefaultViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = new ViewPortOptions
            {
                Width = 456,
                Height = 789
            };

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            Assert.AreEqual(456, await page.EvaluateExpressionAsync<int>("window.innerWidth"));
            Assert.AreEqual(789, await page.EvaluateExpressionAsync<int>("window.innerHeight"));
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should disable the default viewport")]
        public async Task ShouldDisableTheDefaultViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            Assert.Null(page.Viewport);
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should take fullPage screenshots when defaultViewport is null")]
        public async Task ShouldTakeFullPageScreenshotsWhenDefaultViewportIsNull()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.IsNotEmpty(await page.ScreenshotDataAsync(new ScreenshotOptions { FullPage = true }));
        }

        [Test, Retry(2)]
        public async Task ShouldSupportCustomWebSocket()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var customSocketCreated = false;
            options.WebSocketFactory = (uri, socketOptions, cancellationToken) =>
            {
                customSocketCreated = true;
                return WebSocketTransport.DefaultWebSocketFactory(uri, socketOptions, cancellationToken);
            };

            await using (await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customSocketCreated);
            }
        }

        [Test, Retry(2)]
        public async Task ShouldSupportCustomTransport()
        {
            var customTransportCreated = false;
            var options = TestConstants.DefaultBrowserOptions();
            options.TransportFactory = (url, opt, cancellationToken) =>
            {
                customTransportCreated = true;
                return WebSocketTransport.DefaultTransportFactory(url, opt, cancellationToken);
            };

            await using (await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                Assert.True(customTransportCreated);
            }
        }
    }
}
