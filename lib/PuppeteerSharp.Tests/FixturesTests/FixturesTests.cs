using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FixturesTests
{
    public class FixturesTests : PuppeteerBaseTest
    {
        [Test, Retry(2), PuppeteerTest("fixtures.spec", "Fixtures", "should dump browser process stderr")]
        public void ShouldDumpBrowserProcessStderr()
        {
            var success = false;
            var browser = TestConstants.IsChrome ? "chrome" : "firefox";
            using var browserFetcher = new BrowserFetcher(SupportedBrowser.Chrome);
            using var process = GetTestAppProcess(
                "PuppeteerSharp.Tests.DumpIO",
                $"\"{browserFetcher.GetInstalledBrowsers().First().GetExecutablePath()}\" \"${browser}\"");

            process.ErrorDataReceived += (_, e) =>
            {
                success |= e.Data != null && e.Data.Contains("DevTools listening on ws://");
            };

            process.Start();
            process.BeginErrorReadLine();
            process.WaitForExit();
            Assert.True(success);
        }

        public async Task ShouldCloseTheBrowserWhenTheConnectedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            using var browserFetcher = new BrowserFetcher(SupportedBrowser.Chrome);
            var chromiumLauncher = new ChromeLauncher(
                browserFetcher.GetInstalledBrowsers().First().GetExecutablePath(),
                new LaunchOptions { Headless = true });

            await chromiumLauncher.StartAsync().ConfigureAwait(false);

            var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = chromiumLauncher.EndPoint
            });

            browser.Disconnected += (_, _) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(chromiumLauncher.Process.Id);

            await browserClosedTaskWrapper.Task;
            Assert.True(browser.IsClosed);
        }

        [Test, Retry(2), PuppeteerTest("fixtures.spec", "Fixtures", "should close the browser when the node process closes")]
        public async Task ShouldCloseTheBrowserWhenTheLaunchedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    Browser = TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
                },
                TestConstants.LoggerFactory);

            browser.Disconnected += (_, _) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(browser.Process.Id);

            await browserClosedTaskWrapper.Task;
            Assert.True(browser.IsClosed);
        }

        private void KillProcess(int pid)
        {
            using var process = new Process();

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
                "net8.0");
#else
            return Path.Combine(
                TestUtils.FindParentDirectory("lib"),
                dir,
                "bin",
                build,
                "net48");
#endif
        }
    }
}
