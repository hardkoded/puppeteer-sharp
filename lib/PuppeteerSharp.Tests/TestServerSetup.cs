using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.TestServer;

namespace PuppeteerSharp.Tests
{
    [SetUpFixture]
    public class TestServerSetup
    {
        public static SimpleServer Server { get; private set; }
        public static SimpleServer HttpsServer { get; private set; }

        [OneTimeSetUp]
        public async Task InitAllAsync()
        {
            if (OperatingSystem.IsLinux())
            {
                DisableCoreDumps();
            }

            var browserFetcher = new BrowserFetcher(
                TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox
            );
            var downloaderTask = browserFetcher.DownloadAsync();

            Server = SimpleServer.Create(
                0,
                TestUtils.FindParentDirectory("PuppeteerSharp.TestServer")
            );
            HttpsServer = SimpleServer.CreateHttps(
                0,
                TestUtils.FindParentDirectory("PuppeteerSharp.TestServer")
            );

            var serverStart = Server.StartAsync();
            var httpsServerStart = HttpsServer.StartAsync();

            await Task.WhenAll(downloaderTask, serverStart, httpsServerStart);

            TestConstants.Initialize(Server, HttpsServer);
        }

        [OneTimeTearDown]
        public Task ShutDownAsync() => Task.WhenAll(Server.StopAsync(), HttpsServer.StopAsync());

        private static void DisableCoreDumps()
        {
            try
            {
                // Prevent systemd-coredump / apport from compressing multi-GB renderer
                // core dumps during intentional crash tests (e.g. chrome://crash).
                // The browsers launched by this process inherit the limit.
                using var process = Process.Start(
                    "prlimit",
                    ["--core=0:", $"--pid={Environment.ProcessId}"]
                );
                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                // Ignore if prlimit is unavailable.
            }
        }
    }
}
