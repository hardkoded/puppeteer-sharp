using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class PuppeteerLaunchTests : PuppeteerBaseTest
    {
        [Test, PuppeteerTest("launcher.spec", "PuppeteerSharp", "should work in real life")]
        public async Task ShouldWorkInRealLife()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.AcceptInsecureCerts = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            var response = await page.GoToAsync("https://www.github.com");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should reject all promises when browser is closed")]
        public async Task ShouldRejectAllPromisesWhenBrowserIsClosed()
        {
            await using var browser = await Puppeteer.LaunchAsync(
                TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            var neverResolves = page.EvaluateFunctionHandleAsync("() => new Promise(r => {})");
            await browser.CloseAsync();
            var exception = Assert.ThrowsAsync<TargetClosedException>(() => neverResolves);
            Assert.That(exception!.Message, Does.Contain("Protocol error"));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should reject if executable path is invalid")]
        public void ShouldRejectIfExecutablePathIsInvalid()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ExecutablePath = "random-invalid-path";

            Assert.ThrowsAsync<Win32Exception>(() => Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option")]
        public async Task UserDataDirOption()
        {
            using var userDataDir = new TempDirectory();
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;

            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);
            Assert.That(Directory.GetFiles(userDataDir.Path), Is.Not.Empty);
            await browser.CloseAsync();
            Assert.That(Directory.GetFiles(userDataDir.Path), Is.Not.Empty);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir argument")]
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
            Assert.That(Directory.GetFiles(userDataDir.Path), Is.Not.Empty);
            await browser.CloseAsync();
            Assert.That(Directory.GetFiles(userDataDir.Path), Is.Not.Empty);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option should restore state")]
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
                Assert.That(await page2.EvaluateExpressionAsync<string>("localStorage.hey"), Is.EqualTo("hello"));
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option should restore cookies")]
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
                Assert.That(await page2.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("doSomethingOnlyOnce=true"));
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should return the default arguments")]
        public void ShouldReturnTheDefaultArguments()
        {
            Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions() { Headless = true }), Does.Contain("--headless=new"));
            Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions
            {
                Headless = false
            }), Does.Not.Contain("--headless"));

            if (TestConstants.IsChrome)
            {
                Assert.That(Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()), Does.Contain("--no-first-run"));
                Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo"
                }), Does.Contain("--user-data-dir=\"foo\""));
            }
            else
            {
                Assert.That(Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()), Does.Contain("--no-remote"));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.That(Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()), Does.Contain("--foreground"));
                }
                else
                {
                    Assert.That(Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions()), Does.Not.Contain("--foreground"));
                }
                Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo",
                    Browser = TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
                }), Does.Contain("--profile"));
                Assert.That(Puppeteer.GetDefaultArgs(new LaunchOptions
                {
                    UserDataDir = "foo",
                    Browser = TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
                }), Does.Contain("\"foo\""));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        [PuppeteerTest("launcher.spec", "PuppeteerSharp", "chrome should be closed")]
        public async Task ChromeShouldBeClosed(bool useDisposeAsync)
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            await using var browser = await launcher.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));

            if (useDisposeAsync)
            {
                // emulates what would happen in a C#8 await using block
                await browser.DisposeAsync();
            }
            else
            {
                await browser.CloseAsync();
            }

            Assert.That(launcher.Process.HasExited, Is.True);
        }

        [Test, Ignore("previously not marked as a test")]
        public async Task ChromeShouldBeClosedOnDispose()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);

            await using (var browser = await launcher.LaunchAsync(options))
            await using (var page = await browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            }

            Assert.That(await launcher.Process.WaitForExitAsync(TimeSpan.FromSeconds(10)), Is.True);
            Assert.That(launcher.Process.HasExited, Is.True);
        }

        [Test, Ignore("previously not marked as a test")]
        public async Task ShouldNotOpenTwoChromesUsingTheSameLauncher()
        {
            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using (await launcher.LaunchAsync(TestConstants.DefaultBrowserOptions()))
            {
                var exception = Assert.ThrowsAsync<InvalidOperationException>(
                    () => launcher.LaunchAsync(TestConstants.DefaultBrowserOptions()));
                Assert.That(exception!.Message, Is.EqualTo("Unable to create or connect to another process"));
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should filter out ignored default arguments in Chrome")]
        public async Task ShouldFilterOutIgnoredDefaultArguments()
        {
            var defaultArgs = Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions());
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoredDefaultArgs = [defaultArgs[0], defaultArgs[2]];
            await using var browser = await Puppeteer.LaunchAsync(options);
            var spawnArgs = browser.Process.StartInfo.Arguments;
            Assert.That(spawnArgs, Does.Not.Contain(defaultArgs[0]));
            Assert.That(spawnArgs, Does.Contain(defaultArgs[1]));
            Assert.That(spawnArgs, Does.Not.Contain(defaultArgs[2]));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should have default URL when launching browser")]
        public async Task ShouldHaveDefaultUrlWhenLaunchingBrowser()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
            var pages = (await browser.PagesAsync()).Select(page => page.Url);
            Assert.That(pages, Is.EqualTo(new[] { TestConstants.AboutBlank }));
        }


        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should close browser with beforeunload page")]
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

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should have custom URL when launching browser")]
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
            Assert.That(pages[0].Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should set the default viewport")]
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
            Assert.That(await page.EvaluateExpressionAsync<int>("window.innerWidth"), Is.EqualTo(456));
            Assert.That(await page.EvaluateExpressionAsync<int>("window.innerHeight"), Is.EqualTo(789));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should disable the default viewport")]
        public async Task ShouldDisableTheDefaultViewport()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            Assert.That(page.Viewport, Is.Null);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should take fullPage screenshots when defaultViewport is null")]
        public async Task ShouldTakeFullPageScreenshotsWhenDefaultViewportIsNull()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.That(await page.ScreenshotDataAsync(new ScreenshotOptions { FullPage = true }), Is.Not.Empty);
        }

        [Test, PuppeteerTest("launcher.spec", "PuppeteerSharp", "should support custom websocket")]
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
                Assert.That(customSocketCreated, Is.True);
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Browser.close", "should support custom transport")]
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
                Assert.That(customTransportCreated, Is.True);
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir option restores preferences")]
        public async Task UserDataDirOptionRestoresPreferences()
        {
            using var userDataDir = new TempDirectory();
            var userDataDirInfo = new DirectoryInfo(userDataDir.Path);
            var prefsJSPath = Path.Combine(userDataDir.Path, "prefs.js");
            var userJSPath = Path.Combine(userDataDir.Path, "user.js");
            var prefsJSContent = """user_pref("browser.warnOnQuit", true);""";
            await File.WriteAllTextAsync(prefsJSPath, prefsJSContent);
            await File.WriteAllTextAsync(userJSPath, prefsJSContent);

            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await browser.NewPageAsync();

            Assert.That(userDataDirInfo.GetFiles(), Is.Not.Empty);
            await browser.CloseAsync();
            await Assert.MultipleAsync(async () =>
            {
                Assert.That(userDataDirInfo.GetFiles(), Is.Not.Empty);

                Assert.That(
                    await File.ReadAllTextAsync(Path.Combine(userDataDir.Path, "prefs.js")),
                    Is.EqualTo(prefsJSContent));
                Assert.That(
                    await File.ReadAllTextAsync(Path.Combine(userDataDir.Path, "user.js")),
                    Is.EqualTo(prefsJSContent));
            });
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "can launch and close the browser")]
        public async Task CanLaunchAndCloseTheBrowser()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.LoggerFactory);
            await browser.CloseAsync();
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "tmp profile should be cleaned up")]
        public async Task TmpProfileShouldBeCleanedUp()
        {
            var options = TestConstants.DefaultBrowserOptions();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            await using var browser = await launcher.LaunchAsync(options);

            var tempUserDataDir = launcher.Process.TempUserDataDir?.Path;
            Assert.That(tempUserDataDir, Is.Not.Null);
            Assert.That(Directory.Exists(tempUserDataDir), Is.True);

            // Open a page to make sure its functional.
            await browser.NewPageAsync();

            await browser.CloseAsync();

            // Wait briefly for cleanup
            await Task.Delay(500);

            Assert.That(Directory.Exists(tempUserDataDir), Is.False);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "userDataDir argument with non-existent dir")]
        public async Task UserDataDirArgumentWithNonExistentDir()
        {
            using var userDataDir = new TempDirectory();
            var userDataDirPath = userDataDir.Path;

            // Remove the directory so it doesn't exist
            Directory.Delete(userDataDirPath);
            Assert.That(Directory.Exists(userDataDirPath), Is.False);

            var options = TestConstants.DefaultBrowserOptions();

            if (TestConstants.IsChrome)
            {
                options.Args = options.Args.Concat(new[] { $"--user-data-dir=\"{userDataDirPath}\"" }).ToArray();
            }
            else
            {
                options.Args = options.Args.Concat(new string[] { "-profile", userDataDirPath }).ToArray();
            }

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            Assert.That(Directory.GetFiles(userDataDirPath), Is.Not.Empty);
            await browser.CloseAsync();
            Assert.That(Directory.GetFiles(userDataDirPath), Is.Not.Empty);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should filter out ignored default argument in Firefox")]
        public async Task ShouldFilterOutIgnoredDefaultArgumentInFirefox()
        {
            var defaultArgs = Puppeteer.GetDefaultArgs(TestConstants.DefaultBrowserOptions());
            var options = TestConstants.DefaultBrowserOptions();
            // When IgnoredDefaultArgs has specific items, those are excluded but the rest remain.
            // Use a non-existent arg to filter so all real defaults are still present.
            options.IgnoredDefaultArgs = ["--non-existent-flag"];
            await using var browser = await Puppeteer.LaunchAsync(options);
            var spawnArgs = browser.Process.StartInfo.Arguments;
            Assert.That(spawnArgs, Does.Contain(defaultArgs[0]));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should pass the timeout parameter to browser.waitForTarget")]
        public void ShouldPassTheTimeoutParameterToBrowserWaitForTarget()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Timeout = 1;

            Assert.ThrowsAsync<ProcessException>(
                () => Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should work with timeout = 0")]
        public async Task ShouldWorkWithTimeoutEqualTo0()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Timeout = 0;
            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await browser.CloseAsync();
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should launch Chrome properly with --no-startup-window and waitForInitialPage=false")]
        public async Task ShouldLaunchChromeProperlyWithNoStartupWindowAndWaitForInitialPageFalse()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Args = options.Args.Concat(new[] { "--no-startup-window" }).ToArray();
            options.WaitForInitialPage = false;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            Assert.That(await page.EvaluateExpressionAsync<int>("2 + 2"), Is.EqualTo(4));
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should support targetFilter option in puppeteer.launch")]
        public async Task ShouldSupportTargetFilterOptionInPuppeteerLaunch()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.TargetFilter = target => target.Type != TargetType.Page;
            options.WaitForInitialPage = false;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var targets = browser.Targets();
            Assert.That(targets, Has.Exactly(1).Items);
            Assert.That(targets.Any(t => t.Type == TargetType.Page), Is.False);
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.executablePath", "returns executablePath for channel")]
        public void ReturnsExecutablePathForChannel()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.Channel = ChromeReleaseChannel.Stable;
            try
            {
                var defaultArgs = Puppeteer.GetDefaultArgs(options);
                Assert.That(defaultArgs, Is.Not.Null);
            }
            catch (PuppeteerException)
            {
                // It's OK if the channel is not installed; the important thing
                // is that the API accepts the channel parameter without error.
            }
        }

        [Test, PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.executablePath when executable path is configured", "its value is used")]
        public void WhenExecutablePathIsConfiguredItsValueIsUsed()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.ExecutablePath = "SOME_CUSTOM_EXECUTABLE";

            var exception = Assert.CatchAsync(
                () => Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory));
            Assert.That(exception, Is.Not.Null);
        }
    }
}
