using PuppeteerSharp.TestServer;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests
{
    public sealed class PuppeteerLoaderFixture : IAsyncLifetime
    {
        public static SimpleServer Server { get; private set; }
        public static SimpleServer HttpsServer { get; private set; }

        Task IAsyncLifetime.InitializeAsync() => SetupAsync();

        Task IAsyncLifetime.DisposeAsync() => Task.WhenAll(Server.StopAsync(), HttpsServer.StopAsync());

        private async Task SetupAsync()
        {
            using var browserFetcher = new BrowserFetcher(TestConstants.IsChrome ? Product.Chrome : Product.Firefox);
            var downloaderTask = browserFetcher.DownloadAsync();

            Server = SimpleServer.Create(TestConstants.Port, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));
            HttpsServer = SimpleServer.CreateHttps(TestConstants.HttpsPort, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            var serverStart = Server.StartAsync();
            var httpsServerStart = HttpsServer.StartAsync();

            await Task.WhenAll(downloaderTask, serverStart, httpsServerStart);
        }
    }
}
