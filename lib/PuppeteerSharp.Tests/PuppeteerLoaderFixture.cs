using PuppeteerSharp.TestServer;
using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerLoaderFixture : IDisposable
    {
        public static SimpleServer Server { get; private set; }
        public static SimpleServer HttpsServer { get; private set; }

        public PuppeteerLoaderFixture()
        {
            SetupAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Task.WaitAll(Server.StopAsync(), HttpsServer.StopAsync());
        }

        private async Task SetupAsync()
        {
            var downloaderTask = Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);

            Server = SimpleServer.Create(TestConstants.Port, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));
            HttpsServer = SimpleServer.CreateHttps(TestConstants.HttpsPort, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            var serverStart = Server.StartAsync();
            var httpsServerStart = HttpsServer.StartAsync();

            await Task.WhenAll(downloaderTask, serverStart, httpsServerStart);
        }
    }
}
