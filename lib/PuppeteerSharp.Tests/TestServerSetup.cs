using System;
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
            using var browserFetcher = new BrowserFetcher(TestConstants.IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox);
            var downloaderTask = browserFetcher.DownloadAsync();

            Server = SimpleServer.Create(TestConstants.Port, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));
            HttpsServer = SimpleServer.CreateHttps(TestConstants.HttpsPort, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            var serverStart = Server.StartAsync();
            var httpsServerStart = HttpsServer.StartAsync();

            await Task.WhenAll(downloaderTask, serverStart, httpsServerStart);
        }

        [OneTimeTearDown]
        public Task ShutDownAsync()
            => Task.WhenAll(Server.StopAsync(), HttpsServer.StopAsync());
    }
}
