using Microsoft.AspNetCore.Hosting;
using PuppeteerSharp.TestServer;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerLoaderFixture : IAsyncLifetime
    {
        private IWebHost _host;

        private async Task StartWebServerAsync()
        {
            var builder = Startup.GetWebHostBuilder();

            builder.UseContentRoot(TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            _host = builder.Build();

            await _host.StartAsync();
        }

        public async Task InitializeAsync()
        {
            var downloaderTask = Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);
            var serverTask = StartWebServerAsync();

            await Task.WhenAll(downloaderTask, serverTask);
        }

        public async Task DisposeAsync()
        {
            await _host.StopAsync();
        }
    }
}
