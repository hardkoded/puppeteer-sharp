using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Transport;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FixturesTests : PuppeteerBaseTest
    {
        public FixturesTests(ITestOutputHelper output) : base(output) { }

        [SkipBrowserFact(skipFirefox: true)]
        public void ShouldDumpBrowserProcessStderr()
        {
            var success = false;
            var process = GetTestAppProcess(
                "PuppeteerSharp.Tests.DumpIO",
                $"\"{new BrowserFetcher(Product.Chrome).RevisionInfo().ExecutablePath}\"");

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
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var ChromiumLauncher = new ChromiumLauncher(
                new BrowserFetcher(Product.Chrome).RevisionInfo().ExecutablePath,
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

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldCloseTheBrowserWhenTheLaunchedProcessCloses()
        {
            var browserClosedTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }, TestConstants.LoggerFactory);

            browser.Disconnected += (_, _) =>
            {
                browserClosedTaskWrapper.SetResult(true);
            };

            KillProcess(browser.Launcher.Process.Id);

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
                "netcoreapp2.2");
#else
            return Path.Combine(
                TestUtils.FindParentDirectory("lib"),
                dir,
                "bin",
                build,
                "net471");
#endif
        }
    }
}
