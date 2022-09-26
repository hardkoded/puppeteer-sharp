using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FixturesTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FixturesTests : PuppeteerBaseTest
    {
        public FixturesTests(ITestOutputHelper output) : base(output) { }

        [PuppeteerTest("fixtures.spec.ts", "Fixtures", "should dump browser process stderr")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldDumpBrowserProcessStderr()
        {
            var success = false;
            using var browserFetcher = new BrowserFetcher(Product.Chrome);
            var process = GetTestAppProcess(
                "PuppeteerSharp.Tests.DumpIO",
                $"\"{(await browserFetcher.GetRevisionInfoAsync().ConfigureAwait(false)).ExecutablePath}\"");

            process.ErrorDataReceived += (_, e) =>
            {
                success |= e.Data != null && e.Data.Contains("DevTools listening on ws://");
            };

            process.Start();
            process.BeginErrorReadLine();
            process.WaitForExit();
            Assert.True(success);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldCloseTheBrowserWhenTheConnectedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            using var browserFetcher = new BrowserFetcher(Product.Chrome);
            var ChromiumLauncher = new ChromiumLauncher(
                (await browserFetcher.GetRevisionInfoAsync()).ExecutablePath,
                new LaunchOptions { Headless = true });

            await ChromiumLauncher.StartAsync().ConfigureAwait(false);

            var browser = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = ChromiumLauncher.EndPoint
            });

            browser.Disconnected += (_, _) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(ChromiumLauncher.Process.Id);

            await browserClosedTaskWrapper.Task;
            Assert.True(browser.IsClosed);
        }

        [PuppeteerTest("fixtures.spec.ts", "Fixtures", "should close the browser when the node process closes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldCloseTheBrowserWhenTheLaunchedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }, TestConstants.LoggerFactory);

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
                "net6.0");
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
