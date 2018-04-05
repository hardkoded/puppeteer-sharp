using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerLoaderFixture : IDisposable
    {
        IWebHost _host;

        public PuppeteerLoaderFixture()
        {
            SetupAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        private async Task SetupAsync()
        {
            var downloaderTask = Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);
            var serverTask = StartWebServerAsync();

            await Task.WhenAll(downloaderTask, serverTask);
        }

        private async Task StartWebServerAsync()
        {
            var builder = TestServer.Program.GetWebHostBuilder();

            builder.UseContentRoot(FindParentDirectory("PuppeteerSharp.TestServer"));

            _host = builder.Build();

            await _host.StartAsync();
        }

        public static string FindParentDirectory(string directory)
        {
            var current = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(current, directory)))
            {
                current = Directory.GetParent(current).FullName;
            }
            return Path.Combine(current, directory);
        }
    }
}
