using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Transport;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class FixturesTests : PuppeteerBaseTest
    {
        public FixturesTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ShouldDumpBrowserProcessStderr()
        {
            var success = false;
            var process = GetTestAppProcess(
                "PuppeteerSharp.Tests.DumpIO",
                $"\"{new BrowserFetcher().RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath}\"");

            process.ErrorDataReceived += (sender, e) =>
            {
                success |= e.Data != null && e.Data.Contains("DevTools listening on ws://");
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